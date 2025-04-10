using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMembershipService
    {
        Task<IEnumerable<Membership>> GetAllAsync();
        Task<Membership?> GetByIdAsync(int id);
        Task<Membership> CreateMembershipAsync(Membership membership);
    }
}
