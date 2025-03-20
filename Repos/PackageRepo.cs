using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class PackageRepo : IPackageRepo
    {
        public async Task<bool> AddPackageAsync(Package package)
        {
            return await PackageDAO.Instance.AddPackageAsync(package);
        }

        public async Task<bool> DeletePackageAsync(int packageId)
        {
            return await PackageDAO.Instance.DeletePackageAsync(packageId);
        }

        public Task<List<Package>> GetActivePackagesAsync()
        {
            return PackageDAO.Instance.GetActivePackagesAsync();
        }

        public Task<List<Package>> GetAllPackagesAsync()
        {
            return PackageDAO.Instance.GetAllPackagesAsync();
        }

        public async Task<Package?> GetPackageByIdAsync(int packageId)
        {
            return await PackageDAO.Instance.GetPackageByIdAsync(packageId);
        }

        public async Task<bool> UpdatePackageAsync(Package package)
        {
            return await PackageDAO.Instance.UpdatePackageAsync(package);
        }
    }
}
