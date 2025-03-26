using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class ScheduleDAO
    {
        private static ScheduleDAO instance = null;
        private readonly DataContext _context;

        private ScheduleDAO()
        {
           _context = new DataContext();
        }

        public static ScheduleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScheduleDAO();
                }
                return instance;
            }
        }
        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            if (schedule.StartTime >= schedule.EndTime)
                throw new Exception("StartTime must be earlier than EndTime.");

            if (string.IsNullOrWhiteSpace(schedule.Status))
                schedule.Status = "Active";

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleID == scheduleId);
            if (schedule == null)
                throw new Exception("Schedule not found.");
            return schedule;
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            return await _context.Schedules.ToListAsync();
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            var existingSchedule = await _context.Schedules.FindAsync(schedule.ScheduleID);
            if (existingSchedule == null)
                throw new Exception("Schedule not found.");

            if (schedule.StartTime >= schedule.EndTime)
                throw new Exception("StartTime must be earlier than EndTime.");

            existingSchedule.StartTime = schedule.StartTime;
            existingSchedule.EndTime = schedule.EndTime;
            existingSchedule.Status = schedule.Status;

            _context.Schedules.Update(existingSchedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null)
                throw new Exception("Schedule not found.");

            schedule.Status = "Inactive";

            _context.Schedules.Update(schedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Schedule>> SearchSchedulesByTimeAsync(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                throw new Exception("StartTime must be earlier than EndTime.");

            return await _context.Schedules
                .Where(s => s.StartTime >= startTime && s.EndTime <= endTime)
                .Include(s => s.Program)
                .ToListAsync();
        }
        public async Task<IEnumerable<Schedule>> GetActiveSchedulesAsync()
        {
            return await _context.Schedules
                .Where(s => s.Status == "Active")
                .Include(s => s.Program)
                .Include(s => s.VideoHistory)
                .ToListAsync();
        }

    }
}
