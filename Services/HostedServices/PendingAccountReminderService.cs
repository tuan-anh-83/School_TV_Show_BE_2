using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.HostedServices
{
    public class PendingAccountReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PendingAccountReminderService> _logger;
        private readonly TimeSpan _reminderInterval = TimeSpan.FromHours(1);

        public PendingAccountReminderService(IServiceProvider serviceProvider, ILogger<PendingAccountReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendPendingAccountRemindersAsync();
                await Task.Delay(_reminderInterval, stoppingToken);
            }
        }

        private async Task SendPendingAccountRemindersAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                try
                {
                    DateTime threshold = DateTime.UtcNow.AddHours(-1);
                    var pendingAccounts = await accountService.GetPendingAccountsAsync(threshold);

                    foreach (var account in pendingAccounts)
                    {
                        await emailService.SendOtpReminderEmailAsync(account.Email);
                    }

                    _logger.LogInformation("Reminder emails sent for pending accounts.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while sending reminder emails for pending accounts.");
                }
            }
        }
    }
}
