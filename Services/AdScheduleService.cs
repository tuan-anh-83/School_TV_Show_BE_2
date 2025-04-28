using BOs.Models;
using Repos;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class AdScheduleService : IAdScheduleService
    {
        private readonly IAdScheduleRepo _adScheduleRepo;

        public AdScheduleService(IAdScheduleRepo adScheduleRepo)
        {
            _adScheduleRepo = adScheduleRepo;
        }

        public async Task<IEnumerable<AdSchedule>> GetAllAdSchedulesAsync()
        {
            return await _adScheduleRepo.GetAllAsync();
        }

        public async Task<AdSchedule?> GetAdScheduleByIdAsync(int id)
        {
            return await _adScheduleRepo.GetByIdAsync(id);
        }

        public async Task<bool> CreateAdScheduleAsync(AdSchedule adSchedule)
        {
            await _adScheduleRepo.AddAsync(adSchedule);
            await _adScheduleRepo.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateAdScheduleAsync(AdSchedule adSchedule)
        {
            await _adScheduleRepo.UpdateAsync(adSchedule);
            await _adScheduleRepo.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAdScheduleAsync(int id)
        {
            var existing = await _adScheduleRepo.GetByIdAsync(id);
            if (existing == null) return false;

            await _adScheduleRepo.DeleteAsync(existing.AdScheduleID);
            await _adScheduleRepo.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<AdSchedule>> FilterAdSchedulesAsync(DateTime start, DateTime end)
        {
            var all = await _adScheduleRepo.GetAllAsync();
            return all.Where(a => a.StartTime >= start && a.EndTime <= end);
        }
        public async Task<AdSchedule?> GetLatestAdAsync()
        {
            var allAds = await _adScheduleRepo.GetAllAsync();
            return allAds.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
        }
    }
}
