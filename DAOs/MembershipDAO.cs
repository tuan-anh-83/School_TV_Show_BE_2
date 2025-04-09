using BOs.Data;
using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAOs
{
    public class MembershipDAO
    {
        private static MembershipDAO? instance;
        private readonly DataContext _context;

        private MembershipDAO()
        {
            _context = new DataContext();
        }

        public static MembershipDAO Instance => instance ??= new MembershipDAO();

        public async Task<IEnumerable<Membership>> GetAllAsync()
        {
            return await _context.Memberships.Include(m => m.Package).Include(m => m.Account).ToListAsync();
        }

        public async Task<Membership?> GetByIdAsync(int id)
        {
            return await _context.Memberships.Include(m => m.Package).Include(m => m.Account)
                .FirstOrDefaultAsync(m => m.MembershipID == id);
        }

        public async Task<Membership> AddAsync(Membership membership)
        {
            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();
            return membership;
        }
    }
}
