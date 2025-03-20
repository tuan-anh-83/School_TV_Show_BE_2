using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public interface IPackageRepo
    {
        Task<List<Package>> GetAllPackagesAsync();
        Task<List<Package>> GetActivePackagesAsync();
        Task<Package?> GetPackageByIdAsync(int packageId);
        Task<bool> AddPackageAsync(Package package);
        Task<bool> UpdatePackageAsync(Package package);
        Task<bool> DeletePackageAsync(int packageId);
    }
}
