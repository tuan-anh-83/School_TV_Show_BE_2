using BOs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos.Interface
{
    public interface IAdScheduleRepo
    {
        Task<IEnumerable<AdSchedule>> GetAllAsync();
        Task<AdSchedule?> GetByIdAsync(int id);
        Task AddAsync(AdSchedule adSchedule);
        Task UpdateAsync(AdSchedule adSchedule);
        Task DeleteAsync(int adScheduleId);
        Task<IEnumerable<AdSchedule>> FilterByDateRangeAsync(DateTime startTime, DateTime endTime);
        Task SaveAsync();
    }
}
