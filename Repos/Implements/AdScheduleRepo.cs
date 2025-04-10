using BOs.Models;
using DAOs;
using Repos.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos.Implements
{
    public class AdScheduleRepo : IAdScheduleRepo
    {
        public async Task<IEnumerable<AdSchedule>> GetAllAsync()
        {
            return await AdScheduleDAO.Instance.GetAllAsync();
        }

        public async Task<AdSchedule?> GetByIdAsync(int id)
        {
            return await AdScheduleDAO.Instance.GetByIdAsync(id);
        }

        public async Task AddAsync(AdSchedule adSchedule)
        {
            await AdScheduleDAO.Instance.AddAsync(adSchedule);
        }

        public async Task UpdateAsync(AdSchedule adSchedule)
        {
            await AdScheduleDAO.Instance.UpdateAsync(adSchedule);
        }

        public async Task DeleteAsync(int adScheduleId)
        {
            await AdScheduleDAO.Instance.DeleteAsync(adScheduleId);
        }

        public async Task<IEnumerable<AdSchedule>> FilterByDateRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await AdScheduleDAO.Instance.FilterByDateRangeAsync(startTime, endTime);
        }
        
        public async Task SaveAsync()
        {
            await AdScheduleDAO.Instance.SaveAsync();
        }
    }
}
