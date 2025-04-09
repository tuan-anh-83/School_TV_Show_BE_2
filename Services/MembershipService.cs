using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepo _membershipRepo;

        public MembershipService(IMembershipRepo membershipRepo)
        {
            _membershipRepo = membershipRepo;
        }

        public async Task<IEnumerable<Membership>> GetAllAsync()
        {
            return await _membershipRepo.GetAllAsync();
        }

        public async Task<Membership?> GetByIdAsync(int id)
        {
            return await _membershipRepo.GetByIdAsync(id);
        }

        public async Task<Membership> CreateMembershipAsync(Membership membership)
        {
            return await _membershipRepo.AddAsync(membership);
        }
    }
}
