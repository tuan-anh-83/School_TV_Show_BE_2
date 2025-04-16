using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class NotificationRepo : INotificationRepo
    {
        public async Task<List<Notification>> GetByAccountIdAsync(int accountId)
        {
            return await NotificationDAO.Instance.GetByAccountIdAsync(accountId);
        }

        public async Task AddAsync(Notification notification)
        {
            await NotificationDAO.Instance.AddAsync(notification);
        }

        public async Task AddManyAsync(IEnumerable<Notification> notifications)
        {
            await NotificationDAO.Instance.AddManyAsync(notifications);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await NotificationDAO.Instance.MarkAsReadAsync(notificationId);
        }
    }
}
