using BOs.Models;
using DAL.DAO;
using Repos.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Implements
{
    public class LiveStreamRepo : ILiveStreamRepo
    {
        private readonly LiveStreamDAO _dao;

        public LiveStreamRepo()
        {
            _dao = LiveStreamDAO.Instance;
        }
        public void UpdateSchedule(Schedule schedule)
        {
            _dao.UpdateSchedule(schedule);
        }
        public async Task<AdSchedule?> GetNextAvailableAdAsync()
        {
            return await _dao.GetNextAvailableAdAsync();
        }

        public async Task<List<Schedule>> GetLateStartCandidatesAsync(DateTime thresholdTime)
        {
            return await _dao.GetLateStartCandidatesAsync(thresholdTime);
        }

        public async Task<bool> AddVideoHistoryAsync(VideoHistory stream)
        {
            return await _dao.AddVideoHistoryAsync(stream);
        }

        public async Task<bool> UpdateVideoHistoryAsync(VideoHistory stream)
        {
            return await _dao.UpdateVideoHistoryAsync(stream);
        }
        public async Task<List<Schedule>> GetWaitingToStartStreamsAsync()
        {
            return await _dao.GetWaitingToStartStreamsAsync();
        }

        public async Task<Program> GetProgramByIdAsync(int id)
        {
            return await _dao.GetProgramByIdAsync(id);
        }

        public async Task<bool> UpdateProgramAsync(Program program)
        {
            return await _dao.UpdateProgramAsync(program);
        }

        public async Task<bool> AddLikeAsync(VideoLike like)
        {
            return await _dao.AddLikeAsync(like);
        }

        public async Task<bool> AddViewAsync(VideoView view)
        {
            return await _dao.AddViewAsync(view);
        }

        public async Task<bool> AddShareAsync(Share share)
        {
            return await _dao.AddShareAsync(share);
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesBySchoolChannelIdAsync(int schoolChannelId)
        {
            return await _dao.GetSchedulesBySchoolChannelIdAsync(schoolChannelId);
        }

        public async Task<VideoHistory> GetVideoHistoryByStreamIdAsync(string cloudflareStreamId)
        {
            return await _dao.GetVideoHistoryByStreamIdAsync(cloudflareStreamId);
        }

        public async Task<VideoHistory> GetLiveStreamByIdAsync(int id)
        {
            return await _dao.GetLiveStreamByIdAsync(id);
        }

        public async Task<IEnumerable<VideoHistory>> GetActiveLiveStreamsAsync()
        {
            return await _dao.GetActiveLiveStreamsAsync();
        }

        public async Task<bool> CreateScheduleAsync(Schedule schedule)
        {
            return await _dao.CreateScheduleAsync(schedule);
        }

        public async Task<bool> CreateProgramAsync(Program program)
        {
            return await _dao.CreateProgramAsync(program);
        }

        public async Task<List<Schedule>> GetPendingSchedulesAsync(DateTime time)
        {
            return await _dao.GetPendingSchedulesAsync(time);
        }

        public async Task<List<Schedule>> GetReadySchedulesAsync(DateTime time)
        {
            return await _dao.GetReadySchedulesAsync(time);
        }

        public async Task<List<Schedule>> GetEndingSchedulesAsync(DateTime time)
        {
            return await _dao.GetEndingSchedulesAsync(time);
        }

        public async Task<VideoHistory?> GetVideoHistoryByIdAsync(int id)
        {
            return await _dao.GetVideoHistoryByIdAsync(id);
        }

        public async Task<int?> GetFallbackAdVideoHistoryIdAsync()
        {
            return await _dao.GetFallbackAdVideoHistoryIdAsync();
        }

        public async Task AddScheduleAsync(Schedule schedule)
        {
            await _dao.AddScheduleAsync(schedule);
        }

        public async Task SaveChangesAsync()
        {
            await _dao.SaveChangesAsync();
        }

        public async Task<VideoHistory> GetRecordedVideoByStreamIdAsync(string streamId)
        {
            return await _dao.GetRecordedVideoByStreamIdAsync(streamId);
        }

        public async Task<List<Schedule>> GetLiveSchedulesAsync()
        {
            return await _dao.GetLiveSchedulesAsync();
        }
        public async Task<List<Schedule>> GetOverdueSchedulesAsync(DateTime currentTime)
        {
            return await _dao.GetOverdueSchedulesAsync(currentTime);
        }

        public async Task<List<Schedule>> GetLateStartSchedulesPastEndTimeAsync(DateTime now)
        {
            return await _dao.GetLateStartCandidatesAsync(now);
        }

        public async Task AddSchedule(Schedule schedule)
        {
            await _dao.AddScheduleAsync(schedule);
        }
        public async Task UpdateAsync(Schedule schedule) 
        {
            await _dao.UpdateAsync(schedule);
        }
        public async Task<VideoHistory?> GetVideoHistoryByProgramIdAsync(int programId)
        {
            return await _dao.GetVideoHistoryByProgramIdAsync(programId);
        }
    }
}
