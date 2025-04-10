﻿using BOs.Models;
using DAOs;
using Repos.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos.Implements
{
    public class ShareRepo : IShareRepo
    {
        public async Task<List<Share>> GetAllSharesAsync()
        {
            return await ShareDAO.Instance.GetAllSharesAsync();
        }

        public async Task<List<Share>> GetAllActiveSharesAsync()
        {
            return await ShareDAO.Instance.GetAllActiveSharesAsync();
        }

        public async Task<Share?> GetShareByIdAsync(int shareId)
        {
            return await ShareDAO.Instance.GetShareByIdAsync(shareId);
        }

        public async Task<bool> AddShareAsync(Share share)
        {
            return await ShareDAO.Instance.AddShareAsync(share);
        }

        public async Task<bool> UpdateShareAsync(Share share)
        {
            return await ShareDAO.Instance.UpdateShareAsync(share);
        }

        public async Task<bool> DeleteShareAsync(int shareId)
        {
            return await ShareDAO.Instance.DeleteShareAsync(shareId);
        }

        public async Task<int> GetTotalSharesForVideoAsync(int videoHistoryId)
        {
            return await ShareDAO.Instance.GetTotalSharesForVideoAsync(videoHistoryId);
        }

        public async Task<int> GetTotalSharesAsync()
        {
            return await ShareDAO.Instance.GetTotalSharesAsync();
        }

        public async Task<Dictionary<int, int>> GetSharesPerVideoAsync()
        {
            return await ShareDAO.Instance.GetSharesPerVideoAsync();
        }
    }
}
