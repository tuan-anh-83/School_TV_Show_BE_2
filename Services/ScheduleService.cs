using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepo _scheduleRepository;

        public ScheduleService(IScheduleRepo scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            if (schedule.StartTime >= schedule.EndTime)
                throw new Exception("StartTime must be earlier than EndTime.");

            return await _scheduleRepository.CreateScheduleAsync(schedule);
        }

        public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
        {
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new Exception("Schedule not found.");
            return schedule;
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            return await _scheduleRepository.GetAllSchedulesAsync();
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            if (schedule.StartTime >= schedule.EndTime)
                throw new Exception("StartTime must be earlier than EndTime.");

            return await _scheduleRepository.UpdateScheduleAsync(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            return await _scheduleRepository.DeleteScheduleAsync(scheduleId);
        }

        public async Task<IEnumerable<Schedule>> SearchSchedulesByTimeAsync(DateTime startTime, DateTime endTime)
        {
            return await _scheduleRepository.SearchSchedulesByTimeAsync(startTime, endTime);
        }

        public async Task<int> CountSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllSchedulesAsync();
            return schedules.Count();
        }

        public async Task<int> CountSchedulesByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new Exception("Status cannot be null or empty.");

            var schedules = await _scheduleRepository.GetAllSchedulesAsync();
            return schedules.Count(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }
        public async Task<Dictionary<string, int>> GetScheduleCountByStatusAsync()
        {
            var schedules = await _scheduleRepository.GetAllSchedulesAsync();
            return schedules
                .GroupBy(s => s.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<IEnumerable<Schedule>> GetActiveSchedulesAsync()
        {
            return await _scheduleRepository.GetActiveSchedulesAsync();
        }
        public async Task<IEnumerable<Schedule>> GetSchedulesByStatusAsync(string status)
        {
            return (await _scheduleRepository.GetAllSchedulesAsync())
                   .Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

    }

}
