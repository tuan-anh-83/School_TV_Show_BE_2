//using BOs.Models;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace Services
//{
//    public class LiveStreamService : ILiveStreamService
//    {
//        private readonly DataContext _context;
//        private readonly HttpClient _httpClient;
//        private readonly ILogger<LiveStreamService> _logger;
//        private readonly CloudflareSettings _cloudflareSettings;

//        public LiveStreamService(
//            TVSContext context,
//            HttpClient httpClient,
//            ILogger<LiveStreamService> logger,
//            IOptions<CloudflareSettings> cloudflareSettings)
//        {
//            _context = context;
//            _httpClient = httpClient;
//            _logger = logger;
//            _cloudflareSettings = cloudflareSettings.Value;

//            _httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", _cloudflareSettings.ApiToken);
//        }

//        public async Task<bool> SaveRecordedVideoFromWebhookAsync(string cloudflareInputUid, string downloadableUrl, string hlsUrl)
//        {
//            var stream = await _context.VideoHistories
//                .FirstOrDefaultAsync(v => v.CloudflareStreamId == cloudflareInputUid);

//            if (stream == null)
//            {
//                _logger.LogWarning("No matching stream found for CloudflareInputUID: {Uid}", cloudflareInputUid);
//                return false;
//            }

//            stream.MP4Url = downloadableUrl;
//            stream.PlaybackUrl = hlsUrl;
//            stream.Status = false;
//            stream.UpdatedAt = DateTime.UtcNow;
//            stream.Type = "Recorded";

//            _context.VideoHistories.Update(stream);
//            return await _context.SaveChangesAsync() > 0;
//        }
//        public async Task<bool> StartLiveStreamAsync(VideoHistory stream)
//        {
//            var program = await _context.Programs.FindAsync(stream.ProgramID);
//            if (program == null)
//            {
//                _logger.LogError("Program not found for ProgramID: {0}", stream.ProgramID);
//                return false;
//            }
//            if (!string.IsNullOrEmpty(program.CloudflareStreamId))
//            {
//                _logger.LogInformation("Reusing CloudflareStreamId for ProgramID: {0}", program.ProgramID);

//                stream.CloudflareStreamId = program.CloudflareStreamId;
//                stream.URL = $"rtmps://live.cloudflare.com:443/live/{program.CloudflareStreamId}";
//                stream.PlaybackUrl = "";
//                stream.CreatedAt = DateTime.UtcNow;
//                stream.Status = true;

//                _context.VideoHistories.Add(stream);
//                return await _context.SaveChangesAsync() > 0;
//            }
//            var payload = new
//            {
//                meta = new { name = stream.Description },
//                recording = new { mode = "automatic" },
//                mode = "push",
//                playback_policy = new[] { "public" }
//            };

//            var jsonPayload = JsonSerializer.Serialize(payload);
//            var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

//            var url = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs";

//            try
//            {
//                _logger.LogInformation("Sending request to Cloudflare Live Input API...");
//                var response = await _httpClient.PostAsync(url, requestContent);

//                var content = await response.Content.ReadAsStringAsync();
//                if (!response.IsSuccessStatusCode)
//                {
//                    _logger.LogError("Failed to create live input. Status: {0}, Error: {1}", response.StatusCode, content);
//                    return false;
//                }

//                var cloudflareResponse = JsonSerializer.Deserialize<CloudflareLiveInputResponse>(content,
//                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

//                if (cloudflareResponse?.Result == null)
//                {
//                    _logger.LogError("Invalid Cloudflare response: {0}", content);
//                    return false;
//                }
//                stream.CloudflareStreamId = cloudflareResponse.Result.Uid;
//                program.CloudflareStreamId = cloudflareResponse.Result.Uid;
//                _context.Programs.Update(program);
//                if (cloudflareResponse.Result.Rtmps != null)
//                {
//                    stream.URL = $"{cloudflareResponse.Result.Rtmps.Url}{cloudflareResponse.Result.Rtmps.StreamKey}";
//                }
//                else
//                {
//                    stream.URL = null;
//                    _logger.LogWarning("RTMPS info missing from Cloudflare response.");
//                }
//                stream.PlaybackUrl = cloudflareResponse.Result.WebRTCPlayback?.Url ?? string.Empty;
//                stream.CreatedAt = DateTime.UtcNow;
//                stream.Status = true;

//                _context.VideoHistories.Add(stream);
//                return await _context.SaveChangesAsync() > 0;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while creating Cloudflare stream input.");
//                return false;
//            }
//        }
//        public async Task<bool> EndStreamAndReturnLinksAsync(VideoHistory stream)
//        {
//            stream.Status = false;
//            stream.Type = "Recorded";
//            stream.UpdatedAt = DateTime.UtcNow;
//            var videosUrl = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs/{stream.CloudflareStreamId}/videos";
//            var response = await _httpClient.GetAsync(videosUrl);

//            if (!response.IsSuccessStatusCode)
//            {
//                _logger.LogError("Failed to fetch Cloudflare video list. Status: {Status}", response.StatusCode);
//                return false;
//            }
//            var json = await response.Content.ReadAsStringAsync();
//            var videoDetails = JsonSerializer.Deserialize<CloudflareVideoListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

//            if (videoDetails?.Result?.FirstOrDefault() == null)
//            {
//                _logger.LogError("No recorded video found for stream {StreamId}", stream.CloudflareStreamId);
//                return false;
//            }
//            var recordedVideo = videoDetails.Result.First();
//            stream.PlaybackUrl = recordedVideo.Playback?.Hls;
//            var downloadUrl = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/{recordedVideo.Uid}/downloads";
//            var downloadResponse = await _httpClient.PostAsync(downloadUrl, null);

//            if (!downloadResponse.IsSuccessStatusCode)
//            {
//                _logger.LogWarning("Failed to initiate MP4 download.");
//            }
//            else
//            {
//                bool isMp4Ready = false;
//                string mp4Url = null;

//                for (int i = 0; i < 6; i++)
//                {
//                    await Task.Delay(TimeSpan.FromSeconds(10));

//                    var statusResponse = await _httpClient.GetAsync(downloadUrl);
//                    if (!statusResponse.IsSuccessStatusCode)
//                        continue;

//                    var statusJson = await statusResponse.Content.ReadAsStringAsync();
//                    var downloadStatus = JsonSerializer.Deserialize<CloudflareDownloadStatusResponse>(statusJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

//                    if (downloadStatus?.Result?.Default?.Status == "ready")
//                    {
//                        mp4Url = downloadStatus.Result.Default.Url;
//                        isMp4Ready = true;
//                        break;
//                    }
//                }

//                if (!string.IsNullOrEmpty(mp4Url))
//                {
//                    stream.MP4Url = mp4Url;
//                }
//            }
//            _context.VideoHistories.Update(stream);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        public async Task<bool> EndLiveStreamAsync(VideoHistory stream)
//        {
//            var deleteUrl = $"https://api.cloudflare.com/client/v4/accounts/{_cloudflareSettings.AccountId}/stream/live_inputs/{stream.CloudflareStreamId}";
//            var deleteResponse = await _httpClient.DeleteAsync(deleteUrl);

//            if (deleteResponse.IsSuccessStatusCode)
//            {
//                _logger.LogInformation("Cloudflare live input deleted.");
//            }
//            else
//            {
//                _logger.LogWarning("Failed to delete Cloudflare live input.");
//            }

//            return true;
//        }
//        public async Task<VideoHistory> GetLiveStreamByIdAsync(int id)
//        {
//            return await _context.VideoHistories
//                .Include(vh => vh.VideoViews)
//                .Include(vh => vh.VideoLikes)
//                .FirstOrDefaultAsync(vh => vh.VideoHistoryID == id);
//        }

//        public async Task<IEnumerable<VideoHistory>> GetActiveLiveStreamsAsync()
//        {
//            return await _context.VideoHistories
//                .AsNoTracking()
//                .Where(vh => vh.Status)
//                .ToListAsync();
//        }

//        public async Task<bool> AddLikeAsync(VideoLike like)
//        {
//            _context.VideoLikes.Add(like);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        public async Task<bool> AddViewAsync(VideoView view)
//        {
//            _context.VideoViews.Add(view);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        public async Task<bool> AddShareAsync(Share share)
//        {
//            _context.Shares.Add(share);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        public async Task<bool> CreateScheduleAsync(Schedule schedule)
//        {
//            _context.Schedules.Add(schedule);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        public async Task<IEnumerable<Schedule>> GetSchedulesBySchoolChannelIdAsync(int schoolChannelId)
//        {
//            return await _context.Schedules
//                .AsNoTracking()
//                .Where(s => s.Program.SchoolChannel.SchoolChannelID == schoolChannelId)
//                .ToListAsync();
//        }

//        public async Task<bool> CreateProgramAsync(Program program)
//        {
//            _context.Programs.Add(program);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        // DTO classes
//        private class CloudflareDownloadStatusResponse
//        {
//            public CloudflareDownloadResult Result { get; set; }
//            public bool Success { get; set; }
//        }

//        private class CloudflareDownloadResult
//        {
//            public CloudflareDownloadDefault Default { get; set; }
//        }

//        private class CloudflareDownloadDefault
//        {
//            public string Status { get; set; }
//            public string Url { get; set; }
//            public float PercentComplete { get; set; }
//        }

//        private class CloudflareLiveInputResponse
//        {
//            public CloudflareLiveInputResult Result { get; set; }
//            public bool Success { get; set; }
//        }

//        private class CloudflareLiveInputResult
//        {
//            public string Uid { get; set; }
//            public CloudflarePlayback Playback { get; set; }
//            public CloudflareRtmps Rtmps { get; set; }
//            public CloudflareWebRTCPlayback WebRTCPlayback { get; set; }
//        }

//        private class CloudflareWebRTCPlayback
//        {
//            public string Url { get; set; }
//        }

//        private class CloudflarePlayback
//        {
//            public string Hls { get; set; }
//            public string Dash { get; set; }
//        }

//        private class CloudflareRtmps
//        {
//            public string Url { get; set; }
//            public string StreamKey { get; set; }
//        }

//        private class CloudflareVideoListResponse
//        {
//            public List<CloudflareVideoResult> Result { get; set; }
//        }

//        private class CloudflareVideoResult
//        {
//            public string Uid { get; set; }
//            public CloudflarePlayback Playback { get; set; }
//            public string DownloadableUrl { get; set; }
//        }
//    }
//}
