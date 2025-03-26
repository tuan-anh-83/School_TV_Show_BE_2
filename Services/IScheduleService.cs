using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IScheduleService
    {
        Task<Schedule> CreateScheduleAsync(Schedule schedule);
        Task<Schedule> GetScheduleByIdAsync(int scheduleId);
        Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
        Task<bool> UpdateScheduleAsync(Schedule schedule);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task<int> CountSchedulesAsync();
        Task<int> CountSchedulesByStatusAsync(string status);
        Task<Dictionary<string, int>> GetScheduleCountByStatusAsync();
        Task<IEnumerable<Schedule>> GetActiveSchedulesAsync();
    }
}
