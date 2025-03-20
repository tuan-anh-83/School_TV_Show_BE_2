using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ScheduleService :IScheduleService
    {
        private readonly IScheduleRepo _scheduleRepository;

        public ScheduleService(IScheduleRepo scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            if (schedule.StartTime >= schedule.EndTime)
                throw new System.Exception("StartTime must be earlier than EndTime.");

            return await _scheduleRepository.CreateScheduleAsync(schedule);
        }

        public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
        {
            return await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            return await _scheduleRepository.GetAllSchedulesAsync();
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            if (schedule.StartTime >= schedule.EndTime)
                throw new System.Exception("StartTime must be earlier than EndTime.");

            return await _scheduleRepository.UpdateScheduleAsync(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            return await _scheduleRepository.DeleteScheduleAsync(scheduleId);
        }

    }
}
