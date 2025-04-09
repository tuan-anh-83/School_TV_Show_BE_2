using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class ProgramFollowDAO
    {
        private static ProgramFollowDAO? instance = null;
        private readonly DataContext _context;

        private ProgramFollowDAO()
        {
            _context = new DataContext();
        }

        public static ProgramFollowDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProgramFollowDAO();
                }
                return instance;
            }
        }

        public async Task<List<ProgramFollow>> GetAllAsync()
        {
            return await _context.ProgramFollows
                .Include(pf => pf.Account)
                .Include(pf => pf.Program)
                .ToListAsync();
        }

        public async Task<ProgramFollow?> GetByIdAsync(int id)
        {
            return await _context.ProgramFollows
                .Include(pf => pf.Account)
                .Include(pf => pf.Program)
                .FirstOrDefaultAsync(pf => pf.ProgramFollowID == id);
        }

        public async Task<List<ProgramFollow>> GetByAccountIdAsync(int accountId)
        {
            return await _context.ProgramFollows
                .Where(pf => pf.AccountID == accountId)
                .Include(pf => pf.Program)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(ProgramFollow programFollow)
        {
            await _context.ProgramFollows.AddAsync(programFollow);
            return await SaveAsync();
        }

        public async Task<bool> UpdateAsync(ProgramFollow programFollow)
        {
            var existing = await GetByIdAsync(programFollow.ProgramFollowID);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(programFollow);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var programFollow = await GetByIdAsync(id);
            if (programFollow == null) return false;

            _context.ProgramFollows.Remove(programFollow);
            return await SaveAsync();
        }

        public async Task<int> CountByProgramAsync(int programId)
        {
            return await _context.ProgramFollows.CountAsync(pf => pf.ProgramID == programId);
        }

        public async Task<ProgramFollow?> GetByAccountAndProgramAsync(int accountId, int programId)
        {
            return await _context.ProgramFollows
                .FirstOrDefaultAsync(pf => pf.AccountID == accountId && pf.ProgramID == programId);
        }

        public async Task<ProgramFollow> CreateProgramFollowAsync(ProgramFollow follow)
        {
            await _context.ProgramFollows.AddAsync(follow);
            await _context.SaveChangesAsync();
            return follow;
        }

        public async Task<ProgramFollow> UpdateProgramFollowAsync(ProgramFollow follow)
        {
            _context.ProgramFollows.Update(follow);
            await _context.SaveChangesAsync();
            return follow;
        }

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
