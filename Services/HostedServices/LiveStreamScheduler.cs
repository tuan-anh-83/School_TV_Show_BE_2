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
namespace Services.HostedServices
{
    public class LiveStreamScheduler : BackgroundService
    {
        private readonly ILogger<LiveStreamScheduler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public LiveStreamScheduler(ILogger<LiveStreamScheduler> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
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

                try
                {
                    // STEP 1: Mark "Pending" as "Ready"
                    var pending = await repository.GetPendingSchedulesAsync(currentTime.AddMinutes(1));
                    foreach (var s in pending)
                    {
                        s.Status = "Ready";
                        _logger.LogInformation("Schedule {ScheduleID} marked as READY.", s.ScheduleID);
                    }

                    // STEP 2: LateStart detection
                    var lateStartThreshold = currentTime.AddMinutes(-2);
                    var lateCandidates = await repository.GetLateStartCandidatesAsync(lateStartThreshold);
                    foreach (var schedule in lateCandidates)
                    {
                        if (!schedule.LiveStreamStarted && schedule.Status == "Ready")
                        {
                            schedule.Status = "LateStart";
                            await repository.UpdateScheduleAsync(schedule);
                            _logger.LogWarning("Schedule {ScheduleID} marked as LateStart (no live signal after 2 mins)", schedule.ScheduleID);
                        }
                    }
                    await repository.SaveChangesAsync();
                    _logger.LogInformation("LateStart schedules saved.");

                    // STEP 3: Start livestream if signal is active
                    var toStart = await repository.GetReadySchedulesAsync(currentTime);
                    foreach (var schedule in toStart)
                    {
                        if (schedule.Program?.CloudflareStreamId != null)
                        {
                            var isLive = await streamService.IsStreamLiveAsync(schedule.Program.CloudflareStreamId);
                            if (isLive)
                            {
                                var video = new VideoHistory
                                {
                                    ProgramID = schedule.ProgramID,
                                    Description = $"[Live] Program {schedule.Program?.ProgramName}",
                                    CreatedAt = currentTime,
                                    UpdatedAt = currentTime,
                                    StreamAt = schedule.StartTime,
                                    Status = true,
                                    Type = "Live",
                                    CloudflareStreamId = schedule.Program.CloudflareStreamId
                                };

                                bool started = await streamService.StartLiveStreamAsync(video);
                                if (started)
                                {
                                    schedule.VideoHistory = video;
                                    schedule.LiveStreamStarted = true;

                                    // Keep LateStart status if already late
                                    if (schedule.Status != "LateStart")
                                        schedule.Status = "Live";

                                    _logger.LogInformation("Livestream STARTED for ProgramID {ProgramID}, ScheduleID {ScheduleID}, Time: {Time}",
                                        schedule.ProgramID, schedule.ScheduleID, DateTime.UtcNow);
                                }
                                else
                                {
                                    _logger.LogError("Failed to start livestream for Schedule {ScheduleID}", schedule.ScheduleID);
                                }
                            }
                        }
                    }

                    // STEP 4: End stream and insert ad
                    var toEnd = await repository.GetEndingSchedulesAsync(currentTime);
                    foreach (var schedule in toEnd)
                    {
                        if (schedule.VideoHistoryID != null)
                        {
                            var vh = await repository.GetVideoHistoryByIdAsync(schedule.VideoHistoryID.Value);
                            if (vh != null)
                            {
                                bool ended = await streamService.EndStreamAndReturnLinksAsync(vh);
                                schedule.LiveStreamEnded = true;

                                if (!ended || string.IsNullOrEmpty(vh.PlaybackUrl))
                                {
                                    schedule.Status = "EndedEarly";
                                    _logger.LogWarning("EARLY END: No video found for Schedule {ScheduleID}.", schedule.ScheduleID);
                                }
                                else
                                {
                                    schedule.Status = "Ended";
                                    _logger.LogInformation("Live ended normally: Schedule {ScheduleID}", schedule.ScheduleID);
                                }

                                var ad = await adService.GetLatestAdAsync();
                                if (ad != null)
                                {
                                    var adSchedule = new Schedule
                                    {
                                        ProgramID = schedule.ProgramID,
                                        StartTime = DateTime.UtcNow,
                                        EndTime = DateTime.UtcNow.AddMinutes(1),
                                        Mode = "replay",
                                        Status = "Active"
                                    };

                                    var adVideo = new VideoHistory
                                    {
                                        ProgramID = schedule.ProgramID,
                                        CreatedAt = DateTime.UtcNow,
                                        UpdatedAt = DateTime.UtcNow,
                                        StreamAt = DateTime.UtcNow,
                                        Description = "[Fallback Ad] " + ad.Title,
                                        Status = true,
                                        Type = "Recorded",
                                        MP4Url = ad.VideoUrl,
                                        PlaybackUrl = ad.VideoUrl
                                    };

                                    await streamService.CreateScheduleAsync(adSchedule);
                                    await streamService.AddViewAsync(new VideoView { VideoHistory = adVideo });

                                    _logger.LogInformation("Fallback ad inserted after Schedule {ScheduleID}", schedule.ScheduleID);
                                }
                                else
                                {
                                    _logger.LogWarning("No fallback ad available to insert.");
                                }
                            }
                        }
                    }

                    await repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during LiveStreamScheduler tick");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("LiveStreamScheduler stopped.");
        }
    }
}
