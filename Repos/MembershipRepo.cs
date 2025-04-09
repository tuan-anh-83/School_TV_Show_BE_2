using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class MembershipRepo : IMembershipRepo
    {
        public async Task<IEnumerable<Membership>> GetAllAsync()
        {
            return await MembershipDAO.Instance.GetAllAsync();
        }

        public async Task<Membership?> GetByIdAsync(int id)
        {
            return await MembershipDAO.Instance.GetByIdAsync(id);
        }

        public async Task<Membership> AddAsync(Membership membership)
        {
            return await MembershipDAO.Instance.AddAsync(membership);
        }
    }
}
