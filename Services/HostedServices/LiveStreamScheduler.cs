using BOs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using Repos;
using Microsoft.AspNetCore.SignalR;
using Services.Email;
using Services.Hubs;

namespace Services.HostedServices
{
    public class LiveStreamScheduler : BackgroundService
    {
        private readonly ILogger<LiveStreamScheduler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<LiveStreamHub> _hubContext;

        public LiveStreamScheduler(
            ILogger<LiveStreamScheduler> logger,
            IServiceScopeFactory scopeFactory,
            IHubContext<LiveStreamHub> hubContext)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LiveStreamScheduler is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.UtcNow;

                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ILiveStreamRepo>();
                var streamService = scope.ServiceProvider.GetRequiredService<ILiveStreamService>();
                var adService = scope.ServiceProvider.GetRequiredService<IAdScheduleService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                try
                {
                    var pending = await repository.GetPendingSchedulesAsync(currentTime.AddMinutes(1));
                    foreach (var s in pending)
                    {
                        s.Status = "Ready";
                        _logger.LogInformation("Schedule {ScheduleID} marked as READY.", s.ScheduleID);
                    }

                    var lateSchedules = await repository.GetLateStartCandidatesAsync(currentTime);
                    foreach (var s in lateSchedules)
                    {
                        if (!s.LiveStreamStarted && s.Status == "Ready" && currentTime >= s.StartTime.AddMinutes(2))
                        {
                            s.Status = "LateStart";
                            repository.UpdateSchedule(s);
                            _logger.LogWarning("Schedule {ScheduleID} marked as LateStart at {CurrentTime}.", s.ScheduleID, currentTime);
                        }
                    }

                    await repository.SaveChangesAsync();

                    var toStart = await repository.GetReadySchedulesAsync(currentTime);
                    foreach (var schedule in toStart)
                    {
                        var existingVideo = await repository.GetVideoHistoryByProgramIdAsync(schedule.ProgramID);
                        if (existingVideo == null)
                        {
                            var video = new VideoHistory
                            {
                                ProgramID = schedule.ProgramID,
                                Description = $"Scheduled stream for program {schedule.Program?.ProgramName}",
                                CreatedAt = currentTime,
                                UpdatedAt = currentTime,
                                StreamAt = schedule.StartTime,
                                Status = true,
                                Type = "Live",
                                CloudflareStreamId = schedule.Program?.CloudflareStreamId
                            };

                            bool created = await streamService.StartLiveStreamAsync(video);
                            if (created)
                            {
                                schedule.Status = schedule.Status != "LateStart" ? "Ready" : "LateStart";
                                schedule.LiveStreamStarted = false;

                                var streamerEmail = schedule.Program?.SchoolChannel?.Email;
                                if (!string.IsNullOrEmpty(streamerEmail))
                                {
                                    var schoolName = schedule.Program?.SchoolChannel?.Name ?? "School TV Show";
                                    await emailService.SendStreamKeyEmailAsync(
                                        streamerEmail,
                                        video.URL,
                                        schedule.StartTime,
                                        schedule.EndTime,
                                        schoolName
                                    );

                                    _logger.LogInformation("Stream URL created and email sent to {Email} for ScheduleID {ScheduleID}.", streamerEmail, schedule.ScheduleID);
                                }
                                else
                                {
                                    _logger.LogWarning("No email found for ScheduleID {ScheduleID}'s streamer.", schedule.ScheduleID);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Failed to create stream URL for ScheduleID {ScheduleID}", schedule.ScheduleID);
                            }
                        }
                    }

                    await repository.SaveChangesAsync();

                    var waitingSchedules = await repository.GetWaitingToStartStreamsAsync();
                    foreach (var schedule in waitingSchedules)
                    {
                        var videoHistory = await repository.GetVideoHistoryByProgramIdAsync(schedule.ProgramID);
                        if (videoHistory == null) continue;

                        bool streamerStarted = await streamService.CheckStreamerStartedAsync(videoHistory.CloudflareStreamId);
                        if (streamerStarted && !schedule.LiveStreamStarted)
                        {
                            schedule.LiveStreamStarted = true;
                            schedule.Status = "Live";
                            repository.UpdateSchedule(schedule);
                            _logger.LogInformation("Streamer started broadcasting, ScheduleID {ScheduleID} marked LIVE.", schedule.ScheduleID);

                            await _hubContext.Clients.All.SendAsync("StreamStarted", new
                            {
                                scheduleId = schedule.ScheduleID,
                                videoId = videoHistory.VideoHistoryID,
                                url = videoHistory.URL,
                                playbackUrl = videoHistory.PlaybackUrl
                            });
                        }
                    }

                    await repository.SaveChangesAsync();

                    var liveSchedules = await repository.GetLiveSchedulesAsync();
                    foreach (var schedule in liveSchedules)
                    {
                        var videoHistory = await repository.GetVideoHistoryByProgramIdAsync(schedule.ProgramID);
                        if (videoHistory == null) continue;

                        bool isStillStreaming = await streamService.CheckStreamerStartedAsync(videoHistory.CloudflareStreamId);
                        if (!isStillStreaming && !schedule.LiveStreamEnded)
                        {
                            var success = await streamService.EndStreamAndReturnLinksAsync(videoHistory);
                            if (success)
                            {
                                schedule.Status = "EndedEarly";
                                schedule.LiveStreamEnded = true;
                                await repository.SaveChangesAsync();

                                _logger.LogWarning("Stream ended early for ScheduleID {ScheduleID}, saved recordings.", schedule.ScheduleID);

                                await _hubContext.Clients.All.SendAsync("StreamEnded", new
                                {
                                    scheduleId = schedule.ScheduleID,
                                    videoId = videoHistory.VideoHistoryID
                                });
                            }
                            else
                            {
                                _logger.LogError("Error fetching Cloudflare recording for early-ended ScheduleID {ScheduleID}", schedule.ScheduleID);
                            }
                        }
                    }

                    var overdueSchedules = await repository.GetOverdueSchedulesAsync(currentTime);
                    foreach (var schedule in overdueSchedules)
                    {
                        if (schedule.LiveStreamStarted && !schedule.LiveStreamEnded)
                        {
                            var videoHistory = await repository.GetVideoHistoryByProgramIdAsync(schedule.ProgramID);
                            if (videoHistory == null) continue;

                            var success = await streamService.EndStreamAndReturnLinksAsync(videoHistory);
                            if (success)
                            {
                                schedule.Status = "Ended";
                                schedule.LiveStreamEnded = true;
                                await repository.SaveChangesAsync();

                                _logger.LogInformation("Force-ended overdue livestream for ScheduleID {ScheduleID}, recordings saved.", schedule.ScheduleID);

                                await _hubContext.Clients.All.SendAsync("StreamEnded", new
                                {
                                    scheduleId = schedule.ScheduleID,
                                    videoId = videoHistory.VideoHistoryID
                                });
                            }
                            else
                            {
                                _logger.LogError("Failed to force-end livestream or save recording for ScheduleID {ScheduleID}", schedule.ScheduleID);
                            }
                        }
                    }

                    await CheckAndMarkEndedEarlySchedulesAsync(repository, streamService, currentTime);
                    await repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during LiveStreamScheduler tick");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            _logger.LogInformation("LiveStreamScheduler stopped.");
        }

        private async Task CheckAndMarkEndedEarlySchedulesAsync(ILiveStreamRepo repository, ILiveStreamService streamService, DateTime now)
        {
            var lateSchedules = await repository.GetLateStartSchedulesPastEndTimeAsync(now);

            foreach (var schedule in lateSchedules)
            {
                if (!schedule.LiveStreamEnded && schedule.Status == "LateStart")
                {
                    schedule.Status = "EndedEarly";
                    schedule.LiveStreamEnded = true;
                    await repository.UpdateAsync(schedule);

                    var video = await repository.GetVideoHistoryByProgramIdAsync(schedule.ProgramID);
                    if (video != null && !string.IsNullOrEmpty(video.CloudflareStreamId))
                    {
                        try
                        {
                            var deleted = await streamService.EndLiveStreamAsync(video);
                            if (deleted)
                            {
                                _logger.LogInformation("[Auto-End] Cloudflare input deleted for LateStart ScheduleID {ScheduleID}, StreamID {StreamID}.",
                                    schedule.ScheduleID, video.VideoHistoryID);
                            }
                            else
                            {
                                _logger.LogWarning("[Auto-End] Failed to delete Cloudflare input for LateStart ScheduleID {ScheduleID}, StreamID {StreamID}.",
                                    schedule.ScheduleID, video.VideoHistoryID);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[Auto-End] Error deleting Cloudflare input for ScheduleID {ScheduleID}", schedule.ScheduleID);
                        }
                    }

                    _logger.LogInformation("[Auto-End] Schedule {ScheduleID} marked as EndedEarly (no stream detected).", schedule.ScheduleID);
                }
            }
        }
    }
}
