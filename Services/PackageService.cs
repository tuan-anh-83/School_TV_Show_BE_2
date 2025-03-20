using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepo _packageRepo;
        public PackageService(IPackageRepo packageRepo)
        {

            _packageRepo = packageRepo;
        }

        public async Task<bool> AddPackageAsync(Package package)
        {
            return await _packageRepo.AddPackageAsync(package);
        }

        public async Task<bool> DeletePackageAsync(int packageId)
        {
            return await _packageRepo.DeletePackageAsync(packageId);
        }

        public Task<List<Package>> GetActivePackagesAsync()
        {
            return _packageRepo.GetActivePackagesAsync();
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
            return await _packageRepo.GetAllPackagesAsync();
        }

        public async Task<Package?> GetPackageByIdAsync(int packageId)
        {
            return await _packageRepo.GetPackageByIdAsync(packageId);
        }

        public async Task<bool> UpdatePackageAsync(Package package)
        {
            return await _packageRepo.UpdatePackageAsync(package);
        }
    }
}
