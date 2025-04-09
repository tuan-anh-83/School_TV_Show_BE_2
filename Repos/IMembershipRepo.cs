using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public interface IMembershipRepo
    {
        Task<IEnumerable<Membership>> GetAllAsync();
        Task<Membership?> GetByIdAsync(int id);
        Task<Membership> AddAsync(Membership membership);
    }
}
