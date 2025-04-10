using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAccountService
    {
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task<List<Account>> GetAllAccountsAsync();
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<bool> DeleteAccountAsync(int accountId);
        Task<bool> HardDeleteAccountAsync(int accountId);
        Task<bool> SignUpAsync(Account account);
        Task<bool> UpdateAccountAsync(Account account);
        Task<Account?> LoginAsync(string email, string password);
        Task<Account?> SearchAccountByIdAsync(int accountId);
        Task<bool> AssignRoleAsync(int accountId, int roleId);
        Task<Account?> GetAccountByUsernameAsync(string username);
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<bool> UpdateAccountStatusAsync(int accountId, string status);
        Task<List<Account>> SearchAccountsByNameAsync(string searchTerm);
        Task SavePasswordResetTokenAsync(int accountId, string token, DateTime expiration);
        Task<bool> VerifyPasswordResetTokenAsync(int accountId, string token);
        Task InvalidatePasswordResetTokenAsync(int accountId, string token);
        Task<List<Account>> GetAllPendingSchoolOwnerAsync();
        Task<bool> SaveOtpAsync(string email, string otp, DateTime expiration);
        Task<bool> VerifyOtpAsync(string email, string otp);
        Task InvalidateOtpAsync(string email);
        Task<int> GetUserCountAsync();
        Task<int> GetSchoolOwnerCountAsync();
        Task<List<Account>> GetPendingAccountsAsync(DateTime threshold);
        Task<OtpInfo> GetCurrentOtpAsync(string email);
    }
}
