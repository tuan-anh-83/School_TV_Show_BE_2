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

        public async Task<Dictionary<string, List<Schedule>>> GetSchedulesGroupedTimelineAsync()
        {
            var all = await _scheduleRepository.GetAllSchedulesAsync();

            return new Dictionary<string, List<Schedule>>
            {
                ["Live Now"] = all.Where(s => s.Status == "Live").ToList(),
                ["Upcoming"] = all.Where(s => s.Status == "Pending" || s.Status == "Ready").ToList(),
                ["Replay"] = all.Where(s => s.Status == "Ended" || s.Status == "EndedEarly").ToList()
            };
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByChannelAndDateAsync(int channelId, DateTime date)
        {
            return await _scheduleRepository.GetSchedulesByChannelAndDateAsync(channelId, date);
        }

        public async Task<IEnumerable<Schedule>> GetLiveNowSchedulesAsync()
        {
            return await _scheduleRepository.GetLiveNowSchedulesAsync();
        }

        public async Task<IEnumerable<Schedule>> GetUpcomingSchedulesAsync()
        {
            return await _scheduleRepository.GetUpcomingSchedulesAsync();
        }
        public async Task<List<Schedule>> GetSchedulesByDateAsync(DateTime date)
        {
            return await _scheduleRepository.GetSchedulesByDateAsync(date);
        }
        public async Task<Schedule> CreateReplayScheduleFromVideoAsync(int videoHistoryId, DateTime start, DateTime end)
        {
            var program = await _scheduleRepository.GetProgramByVideoHistoryIdAsync(videoHistoryId);
            if (program == null)
                throw new Exception("Program not found for this video.");

            var schedule = new Schedule
            {
                ProgramID = program.ProgramID,
                StartTime = start,
                EndTime = end,
                Status = "Ready",
                IsReplay = true
            };

            return await _scheduleRepository.CreateScheduleAsync(schedule);
        }
    }

}
