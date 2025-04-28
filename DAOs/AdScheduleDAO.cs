using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class AdScheduleDAO
    {
        private static AdScheduleDAO? instance = null;
        private readonly DataContext _context;

        private AdScheduleDAO()
        {
            _context = new DataContext();
        }

        public static AdScheduleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdScheduleDAO();
                }
                return instance;
            }
        }

        public async Task<List<AdSchedule>> GetAllAsync()
        {
            return await _context.AdSchedules.ToListAsync();
        }

        public async Task<AdSchedule?> GetByIdAsync(int id)
        {
            return await _context.AdSchedules.FindAsync(id);
        }

        public async Task<bool> AddAsync(AdSchedule adSchedule)
        {
            await _context.AdSchedules.AddAsync(adSchedule);
            return await SaveAsync();
        }

        public async Task<bool> UpdateAsync(AdSchedule adSchedule)
        {
            var existingAdSchedule = await GetByIdAsync(adSchedule.AdScheduleID);
            if (existingAdSchedule == null)
                return false;

            _context.Entry(existingAdSchedule).CurrentValues.SetValues(adSchedule);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(int adScheduleId)
        {
            var adSchedule = await GetByIdAsync(adScheduleId);
            if (adSchedule == null)
                return false;

            _context.AdSchedules.Remove(adSchedule);
            return await SaveAsync();
        }

        public async Task<List<AdSchedule>> FilterByDateRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.AdSchedules
                .Where(ad => ad.StartTime >= startTime && ad.EndTime <= endTime)
                .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
