using BOs.Models;
using Microsoft.Extensions.Caching.Distributed;
using Repos.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accRepo;
        private readonly IDistributedCache _cache;

        public AccountService(IAccountRepo accRepo, IDistributedCache cache)
        {
            _accRepo = accRepo;
            _cache = cache;
        }

        public Task<bool> HardDeleteAccountAsync(int accountId) => _accRepo.HardDeleteAccountAsync(accountId);

        public Task<Account?> GetAccountByUsernameAsync(string username) => _accRepo.GetAccountByUsernameAsync(username);

        public Task<Account?> GetAccountByEmailAsync(string email) => _accRepo.GetAccountByEmailAsync(email);

        public Task<Role?> GetRoleByIdAsync(int roleId) => _accRepo.GetRoleByIdAsync(roleId);

        public Task<List<Account>> GetAllAccountsAsync() => _accRepo.GetAllAccountsAsync();

        public Task<Account?> GetAccountByIdAsync(int accountId) => _accRepo.GetAccountByIdAsync(accountId);

        public Task<bool> DeleteAccountAsync(int accountId) => _accRepo.DeleteAccountAsync(accountId);

        public Task<bool> SignUpAsync(Account account) => _accRepo.SignUpAsync(account);

        public Task<bool> UpdateAccountAsync(Account account) => _accRepo.UpdateAccountAsync(account);

        public Task<Account?> LoginAsync(string email, string password) => _accRepo.LoginAsync(email, password);

        public Task<Account?> SearchAccountByIdAsync(int accountId) => _accRepo.SearchAccountByIdAsync(accountId);

        public Task<List<Account>> SearchAccountsByNameAsync(string searchTerm) => _accRepo.SearchAccountsByNameAsync(searchTerm);

        public Task<bool> AssignRoleAsync(int accountId, int roleId) => _accRepo.AssignRoleAsync(accountId, roleId);

        public Task<bool> UpdateAccountStatusAsync(int accountId, string status) => _accRepo.UpdateAccountStatusAsync(accountId, status);

        public Task SavePasswordResetTokenAsync(int accountId, string token, DateTime expiration)
            => _accRepo.SavePasswordResetTokenAsync(accountId, token, expiration);

        public Task<bool> VerifyPasswordResetTokenAsync(int accountId, string token)
            => _accRepo.VerifyPasswordResetTokenAsync(accountId, token);

        public Task InvalidatePasswordResetTokenAsync(int accountId, string token)
            => _accRepo.InvalidatePasswordResetTokenAsync(accountId, token);

        public Task<List<Account>> GetAllPendingSchoolOwnerAsync() => _accRepo.GetAllPendingSchoolOwnerAsync();

        public async Task<bool> SaveOtpAsync(string email, string otp, DateTime expiration)
        {
            var otpInfo = new OtpInfo
            {
                Code = otp,
                Expiration = expiration
            };

            var otpJson = JsonSerializer.Serialize(otpInfo);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expiration
            };

            try
            {
                await _cache.SetStringAsync(GetOtpCacheKey(email), otpJson, options);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var otpJson = await _cache.GetStringAsync(GetOtpCacheKey(email));
            if (string.IsNullOrEmpty(otpJson))
                return false;
            var otpInfo = JsonSerializer.Deserialize<OtpInfo>(otpJson);
            return otpInfo != null && otpInfo.Code == otp;
        }

        public async Task InvalidateOtpAsync(string email)
        {
            await _cache.RemoveAsync(GetOtpCacheKey(email));
        }

        public async Task<OtpInfo> GetCurrentOtpAsync(string email)
        {
            var otpJson = await _cache.GetStringAsync(GetOtpCacheKey(email));
            if (string.IsNullOrEmpty(otpJson))
                return null;
            return JsonSerializer.Deserialize<OtpInfo>(otpJson);
        }

        private string GetOtpCacheKey(string email)
        {
            return $"OTP_{email}";
        }

        public Task<int> GetUserCountAsync()
        {
            return _accRepo.GetUserCountAsync();
        }

        public Task<int> GetSchoolOwnerCountAsync()
        {
            return _accRepo.GetSchoolOwnerCountAsync();
        }

        public async Task<List<Account>> GetPendingAccountsAsync(DateTime threshold)
        {
            return await _accRepo.GetPendingAccountsOlderThanAsync(threshold);
        }
    }
}
