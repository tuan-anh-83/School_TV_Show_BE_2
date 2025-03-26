using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class ShareDAO
    {
        private static ShareDAO instance = null;
        private readonly DataContext _context;

        private ShareDAO()
        {
            _context = new DataContext();
        }

        public static ShareDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ShareDAO();
                }
                return instance;
            }
        }

        public async Task<List<Share>> GetAllSharesAsync()
        {
            return await _context.Shares
                .Include(s => s.VideoHistory)
                .Include(s => s.Account)
                .ToListAsync();
        }

        public async Task<List<Share>> GetAllActiveSharesAsync()
        {
            return await _context.Shares
                .Include(s => s.VideoHistory)
                .Include(s => s.Account)
                .Where(s => s.Quantity > 0)
                .ToListAsync();
        }

        public async Task<Share?> GetShareByIdAsync(int shareId)
        {
            if (shareId <= 0)
                throw new ArgumentException("Share ID must be greater than zero.");

            return await _context.Shares
                .Include(s => s.VideoHistory)
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.ShareID == shareId);
        }

        public async Task<bool> AddShareAsync(Share share)
        {
            if (share == null)
                throw new ArgumentNullException(nameof(share));

            share.Quantity = 1;

            await _context.Shares.AddAsync(share);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateShareAsync(Share share)
        {
            if (share == null || share.ShareID <= 0)
                throw new ArgumentException("Invalid Share data.");

            var existingShare = await GetShareByIdAsync(share.ShareID);
            if (existingShare == null)
                return false;

            _context.Entry(existingShare).CurrentValues.SetValues(share);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShareAsync(int shareId)
        {
            if (shareId <= 0)
                throw new ArgumentException("Share ID must be greater than zero.");

            var share = await GetShareByIdAsync(shareId);
            if (share == null)
                return false;

            share.Quantity = 0;
            _context.Shares.Update(share);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalSharesForVideoAsync(int videoHistoryId)
        {
            if (videoHistoryId <= 0)
                throw new ArgumentException("VideoHistory ID must be greater than zero.");

            return await _context.Shares
                .Where(s => s.VideoHistoryID == videoHistoryId && s.Quantity > 0)
                .SumAsync(s => s.Quantity);
        }

        public async Task<int> GetTotalSharesAsync()
        {
            return await _context.Shares.SumAsync(s => s.Quantity);
        }

        public async Task<Dictionary<int, int>> GetSharesPerVideoAsync()
        {
            return await _context.Shares
                .GroupBy(s => s.VideoHistoryID)
                .Select(g => new { VideoHistoryID = g.Key, TotalShares = g.Sum(s => s.Quantity) })
                .ToDictionaryAsync(x => x.VideoHistoryID, x => x.TotalShares);
        }
    }
}
