//using BOs.Models;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Services;
//namespace Services.HostedServices
//{
//    public class LiveStreamScheduler : BackgroundService
//    {
//        private readonly ILogger<LiveStreamScheduler> _logger;
//        private readonly IServiceScopeFactory _scopeFactory;

//        public LiveStreamScheduler(ILogger<LiveStreamScheduler> logger, IServiceScopeFactory scopeFactory)
//        {
//            _logger = logger;
//            _scopeFactory = scopeFactory;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            _logger.LogInformation("LiveStreamScheduler is starting.");

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                var currentTime = DateTime.UtcNow;

//                using (var scope = _scopeFactory.CreateScope())
//                {
//                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
//                    var streamService = scope.ServiceProvider.GetRequiredService<ILiveStreamService>();

//                    try
//                    {
//                        var liveSchedulesToStart = await dbContext.Schedules
//                            .Include(s => s.Program)
//                            .Where(s => s.StartTime <= currentTime && !s.LiveStreamStarted && s.Status == "Active" && s.Mode == "live")
//                            .ToListAsync(stoppingToken);

//                        foreach (var schedule in liveSchedulesToStart)
//                        {
//                            schedule.LiveStreamStarted = true;

//                            var videoHistory = new VideoHistory
//                            {
//                                ProgramID = schedule.ProgramID,
//                                Description = $"Scheduled Live Stream for Program {schedule.Program.ProgramName}",
//                                CreatedAt = currentTime,
//                                UpdatedAt = currentTime,
//                                StreamAt = schedule.StartTime,
//                                Status = true,
//                                Type = "Live",
//                            };

//                            bool started = await streamService.StartLiveStreamAsync(videoHistory);

//                            if (started)
//                            {
//                                schedule.VideoHistoryID = videoHistory.VideoHistoryID;
//                                _logger.LogInformation($"Started LIVE stream | ScheduleID: {schedule.ScheduleID}");
//                            }
//                            else
//                            {
//                                _logger.LogError($"Failed to start LIVE stream | ScheduleID: {schedule.ScheduleID}");
//                            }
//                        }
//                        var schedulesToEnd = await dbContext.Schedules
//                            .Include(s => s.Program)
//                            .Where(s => s.EndTime <= currentTime && s.LiveStreamStarted && !s.LiveStreamEnded && s.Status == "Active" && s.Mode == "live")
//                            .ToListAsync(stoppingToken);

//                        foreach (var schedule in schedulesToEnd)
//                        {
//                            schedule.LiveStreamEnded = true;

//                            if (schedule.VideoHistoryID.HasValue)
//                            {
//                                var videoHistory = await dbContext.VideoHistories.FindAsync(schedule.VideoHistoryID.Value);

//                                if (videoHistory != null)
//                                {
//                                    bool ended = await streamService.EndLiveStreamAsync(videoHistory);

//                                    if (ended)
//                                    {
//                                        _logger.LogInformation($"Ended LIVE stream | ScheduleID: {schedule.ScheduleID}");

//                                        var fallbackVideoId = await dbContext.VideoHistories
//                                            .Where(v => v.Type == "Recorded" && v.Description.Contains("ad"))
//                                            .Select(v => (int?)v.VideoHistoryID)
//                                            .FirstOrDefaultAsync();

//                                        if (fallbackVideoId.HasValue)
//                                        {
//                                            var adSchedule = new Schedule
//                                            {
//                                                ProgramID = schedule.ProgramID,
//                                                StartTime = DateTime.UtcNow,
//                                                EndTime = DateTime.UtcNow.AddMinutes(5),
//                                                Status = "Active",
//                                                Mode = "replay",
//                                                SourceVideoHistoryID = fallbackVideoId.Value
//                                            };

//                                            await dbContext.Schedules.AddAsync(adSchedule);
//                                            _logger.LogInformation("Fallback ad scheduled after stream.");
//                                        }
//                                    }
//                                    else
//                                    {
//                                        _logger.LogError($"Failed to end stream | ScheduleID: {schedule.ScheduleID}");
//                                    }
//                                }
//                                else
//                                {
//                                    _logger.LogError($"VideoHistory not found | ScheduleID: {schedule.ScheduleID}");
//                                }
//                            }
//                        }

//                        await dbContext.SaveChangesAsync(stoppingToken);
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.LogError(ex, "Error in LiveStreamScheduler");
//                    }
//                }

//                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
//            }

//            _logger.LogInformation("LiveStreamScheduler is stopping.");
//        }
//    }
//}
