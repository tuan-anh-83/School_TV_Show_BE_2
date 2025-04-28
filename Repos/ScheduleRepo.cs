using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class ScheduleRepo : IScheduleRepo
    {
        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            return await ScheduleDAO.Instance.CreateScheduleAsync(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            return await ScheduleDAO.Instance.DeleteScheduleAsync(scheduleId);
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            return await ScheduleDAO.Instance.GetAllSchedulesAsync();
        }

        public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
        {
            return await ScheduleDAO.Instance.GetScheduleByIdAsync(scheduleId);
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            return await ScheduleDAO.Instance.UpdateScheduleAsync(schedule);
        }

        public async Task<IEnumerable<Schedule>> GetActiveSchedulesAsync()
        {
            return await ScheduleDAO.Instance.GetActiveSchedulesAsync();
        }

        public async Task<IEnumerable<Schedule>> GetLiveNowSchedulesAsync()
        {
            return await ScheduleDAO.Instance.GetLiveNowSchedulesAsync();
        }

        public async Task<IEnumerable<Schedule>> GetUpcomingSchedulesAsync()
        {
            return await ScheduleDAO.Instance.GetUpcomingSchedulesAsync();
        }

        public async Task<Dictionary<string, List<Schedule>>> GetSchedulesGroupedTimelineAsync()
        {
            return await ScheduleDAO.Instance.GetSchedulesGroupedTimelineAsync();
        }

        public async Task<List<Schedule>> GetSchedulesByDateAsync(DateTime date)
        {
            return await ScheduleDAO.Instance.GetSchedulesByDateAsync(date);
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByChannelAndDateAsync(int channelId, DateTime date)
        {
            return await ScheduleDAO.Instance.GetSchedulesByChannelAndDateAsync(channelId, date);
        }
        public async Task<Program?> GetProgramByVideoHistoryIdAsync(int videoHistoryId)
        {
            return await ScheduleDAO.Instance.GetProgramByVideoHistoryIdAsync((int)videoHistoryId);
        }
        public async Task<List<Schedule>> GetSchedulesByProgramIdAsync(int programId)
        {
            return await ScheduleDAO.Instance.GetSchedulesByProgramIdAsync((int)programId);
        }
        public async Task<bool> IsScheduleOverlappingAsync(int schoolChannelId, DateTime startTime, DateTime endTime)
        {
            return await ScheduleDAO.Instance.IsScheduleOverlappingAsync(schoolChannelId, startTime, endTime);
        }
        public async Task<Program?> GetProgramByIdAsync(int programId)
        {
            return await ScheduleDAO.Instance.GetProgramByIdAsync((int)programId);
        }
    }
}
