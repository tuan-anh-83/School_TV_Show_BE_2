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
            try
            {
                Console.WriteLine("Attempting to create a new schedule...");
                Console.WriteLine($"StartTime: {schedule.StartTime}, EndTime: {schedule.EndTime}, Status: {schedule.Status}");

                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();

                Console.WriteLine("Schedule created successfully.");
                return schedule;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error occurred while saving the schedule: {dbEx.Message}");
                if (dbEx.InnerException != null)
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");

                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");

                throw;
            }
        }


        public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
        {
            return await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleID == scheduleId);
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            return await _context.Schedules.ToListAsync();
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            var existingSchedule = await _context.Schedules.FindAsync(schedule.ScheduleID);
            if (existingSchedule == null)
                return false;

            existingSchedule.StartTime = schedule.StartTime;
            existingSchedule.EndTime = schedule.EndTime;

            _context.Schedules.Update(existingSchedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null)
                return false;

            schedule.Status = "Inactive";

            _context.Schedules.Update(schedule);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
