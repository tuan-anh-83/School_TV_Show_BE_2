using BOs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Repos;

namespace Services
{
    public class LiveStreamService : ILiveStreamService
    {
        private readonly ILiveStreamRepo _repository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<LiveStreamService> _logger;
        private readonly CloudflareSettings _cloudflareSettings;

        public LiveStreamService(
            ILiveStreamRepo repository,
            HttpClient httpClient,
            ILogger<LiveStreamService> logger,
            IOptions<CloudflareSettings> cloudflareSettings)
        {
            _repository = repository;
            _httpClient = httpClient;
            _logger = logger;
            _cloudflareSettings = cloudflareSettings.Value;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _cloudflareSettings.ApiToken);
        }

        public async Task<bool> CheckStreamerStartedAsync(string cloudflareStreamId)
        {
            var videosUrl = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs/{cloudflareStreamId}/videos";
            var response = await _httpClient.GetAsync(videosUrl);
            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            var videoDetails = System.Text.Json.JsonSerializer.Deserialize<CloudflareVideoListResponse>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return videoDetails?.Result?.Any(video => video.Status?.State == "live-inprogress") ?? false;
        }
        public async Task<bool> SaveRecordedVideoFromWebhookAsync(string cloudflareInputUid, string downloadableUrl, string hlsUrl)
        {
            var existingRecorded = await _repository.GetRecordedVideoByStreamIdAsync(cloudflareInputUid);
            if (existingRecorded != null)
            {
                _logger.LogWarning("Duplicate recording detected. Stream ID {Uid} already has a recorded video.", cloudflareInputUid);
                return false;
            }
            var stream = await _repository.GetVideoHistoryByStreamIdAsync(cloudflareInputUid);
            if (stream == null)
            {
                _logger.LogWarning("No matching stream found for CloudflareInputUID: {Uid}", cloudflareInputUid);
                return false;
            }

            stream.MP4Url = downloadableUrl;
            stream.PlaybackUrl = hlsUrl;
            stream.Status = false;
            stream.UpdatedAt = DateTime.UtcNow;
            stream.Type = "Recorded";

            return await _repository.UpdateVideoHistoryAsync(stream);
        }
        public async Task<bool> StartLiveStreamAsync(VideoHistory stream)
        {
            if (stream.ProgramID == null)
            {
                _logger.LogError("ProgramID is null. Cannot start livestream.");
                return false;
            }

            var program = await _repository.GetProgramByIdAsync(stream.ProgramID.Value);
            if (program == null)
            {
                _logger.LogError("Program not found for ProgramID: {0}", stream.ProgramID);
                return false;
            }

            if (!string.IsNullOrEmpty(program.CloudflareStreamId))
            {
                var checkUrl = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs/{program.CloudflareStreamId}";
                var checkResponse = await _httpClient.GetAsync(checkUrl);

                if (checkResponse.IsSuccessStatusCode)
                {
                    var checkJson = await checkResponse.Content.ReadAsStringAsync();
                    var existingStream = System.Text.Json.JsonSerializer.Deserialize<CloudflareLiveInputResponse>(checkJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var rtmps = existingStream?.Result?.Rtmps;
                    if (rtmps != null)
                    {
                        _logger.LogInformation("Reusing existing CloudflareStreamId for ProgramID: {0}", program.ProgramID);

                        stream.CloudflareStreamId = program.CloudflareStreamId;
                        stream.URL = $"{rtmps.Url}{rtmps.StreamKey}";
                        stream.PlaybackUrl = string.Empty;
                        stream.CreatedAt = DateTime.UtcNow;
                        stream.Status = true;

                        return await _repository.AddVideoHistoryAsync(stream);
                    }
                    else
                    {
                        _logger.LogWarning("Missing RTMPS config in reused stream for ProgramID: {0}", program.ProgramID);
                    }
                }
                else
                {
                    _logger.LogWarning("CloudflareStreamId {0} is invalid. Creating new stream...", program.CloudflareStreamId);
                    program.CloudflareStreamId = null;
                }
            }

            var payload = new
            {
                meta = new { name = stream.Description },
                recording = new { mode = "automatic" },
                mode = "push",
                playback_policy = new[] { "public" }
            };

            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var url = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs";

            try
            {
                var response = await _httpClient.PostAsync(url, requestContent);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to create live input. Status: {0}, Error: {1}", response.StatusCode, content);
                    return false;
                }

                var cloudflareResponse = System.Text.Json.JsonSerializer.Deserialize<CloudflareLiveInputResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (cloudflareResponse?.Result == null)
                {
                    _logger.LogError("Invalid Cloudflare response: {0}", content);
                    return false;
                }

                stream.CloudflareStreamId = cloudflareResponse.Result.Uid;
                program.CloudflareStreamId = cloudflareResponse.Result.Uid;
                await _repository.UpdateProgramAsync(program);

                stream.URL = cloudflareResponse.Result.Rtmps != null
                    ? $"{cloudflareResponse.Result.Rtmps.Url}{cloudflareResponse.Result.Rtmps.StreamKey}"
                    : null;

                stream.PlaybackUrl = cloudflareResponse.Result.WebRTCPlayback?.Url ?? string.Empty;
                stream.CreatedAt = DateTime.UtcNow;
                stream.Status = true;

                return await _repository.AddVideoHistoryAsync(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating Cloudflare stream input.");
                return false;
            }
        }
        public async Task<bool> EndStreamAndReturnLinksAsync(VideoHistory stream)
        {
            stream.Status = false;
            stream.Type = "Recorded";
            stream.UpdatedAt = DateTime.UtcNow;

            var videosUrl = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs/{stream.CloudflareStreamId}/videos";
            var response = await _httpClient.GetAsync(videosUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch Cloudflare video list. Status: {Status}", response.StatusCode);
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            var videoDetails = JsonSerializer.Deserialize<CloudflareVideoListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var recordedVideo = videoDetails?.Result?.FirstOrDefault();
            if (recordedVideo == null)
            {
                _logger.LogWarning("EARLY END DETECTED: No recorded video found for stream {StreamId}.", stream.CloudflareStreamId);
                return false;
            }

            stream.PlaybackUrl = $"https://customer-{_cloudflareSettings.StreamDomain}.cloudflarestream.com/{recordedVideo.Uid}/iframe";
            stream.Duration = recordedVideo.Duration;
            _logger.LogInformation("Stream duration saved: {Duration} seconds", stream.Duration);

            var downloadUrl = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/{recordedVideo.Uid}/downloads";
            var downloadResponse = await _httpClient.PostAsync(downloadUrl, null);

            if (downloadResponse.IsSuccessStatusCode)
            {
                int retryCount = 0;
                const int maxRetries = 5;
                const int delaySeconds = 15;

                while (retryCount < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

                    var statusResponse = await _httpClient.GetAsync(downloadUrl);
                    if (!statusResponse.IsSuccessStatusCode)
                    {
                        retryCount++;
                        continue;
                    }

                    var statusJson = await statusResponse.Content.ReadAsStringAsync();
                    var downloadStatus = JsonSerializer.Deserialize<CloudflareDownloadStatusResponse>(statusJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (downloadStatus?.Result?.Default?.Status == "ready")
                    {
                        stream.MP4Url = downloadStatus.Result.Default.Url;
                        _logger.LogInformation("MP4 download ready: {Url}", stream.MP4Url);
                        break;
                    }
                    retryCount++;
                }
            }
            else
            {
                _logger.LogWarning("Failed to initiate MP4 download.");
            }
            var updateSuccess = await _repository.UpdateVideoHistoryAsync(stream);
            if (!string.IsNullOrEmpty(stream.CloudflareStreamId))
            {
                var deleted = await EndLiveStreamAsync(stream);
                if (deleted)
                    _logger.LogInformation("Cloudflare input deleted successfully for stream {StreamID}", stream.VideoHistoryID);
                else
                    _logger.LogWarning("Failed to delete Cloudflare input for stream {StreamID}", stream.VideoHistoryID);
            }
            return updateSuccess;
        }

        public async Task<bool> EndLiveStreamAsync(VideoHistory stream)
        {
            var deleteUrl = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs/{stream.CloudflareStreamId}";
            var deleteResponse = await _httpClient.DeleteAsync(deleteUrl);

            if (!deleteResponse.IsSuccessStatusCode)
                _logger.LogWarning("Failed to delete Cloudflare live input.");
            else
                _logger.LogInformation("Cloudflare live input deleted.");

            return true;
        }
        public async Task<bool> IsStreamLiveAsync(string cloudflareStreamId)
        {
            var url = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs/{cloudflareStreamId}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return false;

            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<CloudflareLiveInputResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Result?.Status?.State?.ToLower() == "live";
        }
        public async Task<bool> CheckLiveInputExistsAsync(string streamId)
        {
            if (string.IsNullOrEmpty(streamId)) return false;

            try
            {
                var url = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs/{streamId}";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cloudflareSettings.ApiToken);

                var response = await client.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CheckLiveInputExistsAsync] Error checking Cloudflare stream input existence for ID {StreamId}", streamId);
                return false;
            }
        }
        public async Task<VideoHistory> GetLiveStreamByIdAsync(int id) => await _repository.GetLiveStreamByIdAsync(id);
        public async Task<IEnumerable<VideoHistory>> GetActiveLiveStreamsAsync() => await _repository.GetActiveLiveStreamsAsync();
        public async Task<bool> AddLikeAsync(VideoLike like) => await _repository.AddLikeAsync(like);
        public async Task<bool> AddViewAsync(VideoView view) => await _repository.AddViewAsync(view);
        public async Task<bool> AddShareAsync(Share share) => await _repository.AddShareAsync(share);
        public async Task<bool> CreateScheduleAsync(Schedule schedule) => await _repository.CreateScheduleAsync(schedule);
        public async Task<IEnumerable<Schedule>> GetSchedulesBySchoolChannelIdAsync(int schoolChannelId) => await _repository.GetSchedulesBySchoolChannelIdAsync(schoolChannelId);
        public async Task<bool> CreateProgramAsync(Program program) => await _repository.CreateProgramAsync(program);

        #region Corrected Cloudflare Models
        private class CloudflareVideoListResponse
        {
            public List<CloudflareVideoResult> Result { get; set; }
            public bool Success { get; set; }
            public List<object> Errors { get; set; }
            public List<object> Messages { get; set; }
        }

        private class CloudflareVideoResult
        {
            public string Uid { get; set; }
            public CloudflareVideoStatus Status { get; set; }
            public double Duration { get; set; }
        }

        private class CloudflareVideoStatus
        {
            public string State { get; set; }
            public string ErrorReasonCode { get; set; }
            public string ErrorReasonText { get; set; }
        }

        private class CloudflarePlayback
        {
            public string Hls { get; set; }
            public string Dash { get; set; }
        }

        private class CloudflareDownloadStatusResponse { public CloudflareDownloadResult Result { get; set; } public bool Success { get; set; } }
        private class CloudflareDownloadResult { public CloudflareDownloadDefault Default { get; set; } }
        private class CloudflareDownloadDefault { public string Status { get; set; } public string Url { get; set; } public float PercentComplete { get; set; } }

        private class CloudflareLiveInputResponse { public CloudflareLiveInputResult Result { get; set; } public bool Success { get; set; } }
        private class CloudflareLiveInputResult
        {
            public string Uid { get; set; }
            public CloudflareStatus Status { get; set; }
            public CloudflarePlayback Playback { get; set; }
            public CloudflareRtmps Rtmps { get; set; }
            public CloudflareWebRTCPlayback WebRTCPlayback { get; set; }
        }
        private class CloudflareStatus
        {
            public string State { get; set; }
            public string ErrorReasonCode { get; set; }
            public string ErrorReasonText { get; set; }
        }

        private class CloudflareWebRTCPlayback { public string Url { get; set; } }
        private class CloudflareRtmps { public string Url { get; set; } public string StreamKey { get; set; } }
        #endregion
    }
}
