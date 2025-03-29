using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOs.Data;

namespace DAL.DAO
{
    public class LiveStreamDAO
    {
        private static LiveStreamDAO? instance = null;
        private readonly DataContext _context;

        private LiveStreamDAO()
        {
            _context = new DataContext();
        }

        public static LiveStreamDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LiveStreamDAO();
                }
                return instance;
            }
        }

        public async Task<AdSchedule?> GetNextAvailableAdAsync()
        {
            return await _context.AdSchedules
                .OrderBy(a => a.CreatedAt)
                .FirstOrDefaultAsync();
        }


        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            var tracked = _context.Schedules.Local.FirstOrDefault(s => s.ScheduleID == schedule.ScheduleID);
            if (tracked != null)
            {
                tracked.Status = schedule.Status;
                tracked.LiveStreamStarted = schedule.LiveStreamStarted;
                tracked.LiveStreamEnded = schedule.LiveStreamEnded;
                tracked.VideoHistoryID = schedule.VideoHistoryID;
            }
            else
            {
                _context.Schedules.Attach(schedule);
                _context.Entry(schedule).State = EntityState.Modified;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Schedule>> GetLateStartCandidatesAsync(DateTime thresholdTime)
        {
            return await _context.Schedules
                .Where(s => s.Status == "Ready" && !s.LiveStreamStarted && s.StartTime <= thresholdTime)
                .ToListAsync();
        }

        public async Task<bool> AddVideoHistoryAsync(VideoHistory stream)
        {
            _context.VideoHistories.Add(stream);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateVideoHistoryAsync(VideoHistory stream)
        {
            _context.VideoHistories.Update(stream);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Program> GetProgramByIdAsync(int id)
        {
            return await _context.Programs.FindAsync(id);
        }

        public async Task<bool> UpdateProgramAsync(Program program)
        {
            _context.Programs.Update(program);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddLikeAsync(VideoLike like)
        {
            _context.VideoLikes.Add(like);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddViewAsync(VideoView view)
        {
            _context.VideoViews.Add(view);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddShareAsync(Share share)
        {
            _context.Shares.Add(share);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesBySchoolChannelIdAsync(int schoolChannelId)
        {
            return await _context.Schedules
                .AsNoTracking()
                .Where(s => s.Program.SchoolChannel.SchoolChannelID == schoolChannelId)
                .ToListAsync();
        }

        public async Task<VideoHistory> GetVideoHistoryByStreamIdAsync(string cloudflareStreamId)
        {
            return await _context.VideoHistories
                .FirstOrDefaultAsync(v => v.CloudflareStreamId == cloudflareStreamId);
        }

        public async Task<VideoHistory> GetLiveStreamByIdAsync(int id)
        {
            return await _context.VideoHistories
                .Include(vh => vh.VideoViews)
                .Include(vh => vh.VideoLikes)
                .FirstOrDefaultAsync(vh => vh.VideoHistoryID == id);
        }

        public async Task<IEnumerable<VideoHistory>> GetActiveLiveStreamsAsync()
        {
            return await _context.VideoHistories
                .AsNoTracking()
                .Where(vh => vh.Status)
                .ToListAsync();
        }

        public async Task<bool> CreateScheduleAsync(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateProgramAsync(Program program)
        {
            _context.Programs.Add(program);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Schedule>> GetPendingSchedulesAsync(DateTime time)
        {
            return await _context.Schedules
                .Where(s => s.Status == "Pending" && s.Mode == "live" && s.StartTime <= time)
                .ToListAsync();
        }

        public async Task<List<Schedule>> GetReadySchedulesAsync(DateTime time)
        {
            return await _context.Schedules
                .Include(s => s.Program)
                .Where(s => s.Status == "Ready" && !s.LiveStreamStarted && s.StartTime <= time && s.Mode == "live")
                .ToListAsync();
        }

        public async Task<List<Schedule>> GetEndingSchedulesAsync(DateTime time)
        {
            return await _context.Schedules
                .Include(s => s.Program)
                .Where(s => s.LiveStreamStarted && !s.LiveStreamEnded && s.EndTime <= time && s.Mode == "live")
                .ToListAsync();
        }

        public async Task<VideoHistory?> GetVideoHistoryByIdAsync(int id)
        {
            return await _context.VideoHistories.FindAsync(id);
        }

        public async Task<int?> GetFallbackAdVideoHistoryIdAsync()
        {
            return await _context.VideoHistories
                .Where(v => v.Type == "Recorded" && v.Description.Contains("ad"))
                .Select(v => (int?)v.VideoHistoryID)
                .FirstOrDefaultAsync();
        }

        public async Task AddScheduleAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<VideoHistory> GetRecordedVideoByStreamIdAsync(string streamId)
        {
            return await _context.VideoHistories
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.CloudflareStreamId == streamId && v.Type == "Recorded");
        }
    }
}
