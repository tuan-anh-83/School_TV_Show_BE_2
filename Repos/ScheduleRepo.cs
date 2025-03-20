using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class ScheduleRepo : IScheduleRepo
    {
        public Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            return  ScheduleDAO.Instance.CreateScheduleAsync(schedule);
        }

        public Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            return  ScheduleDAO.Instance.DeleteScheduleAsync(scheduleId);
        }

        public Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            return  ScheduleDAO.Instance.GetAllSchedulesAsync();
        }

        public Task<Schedule> GetScheduleByIdAsync(int scheduleId)
        {
            return  ScheduleDAO.Instance.GetScheduleByIdAsync(scheduleId);
        }

        public Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            return  ScheduleDAO.Instance.UpdateScheduleAsync(schedule);
        }
    }
}
