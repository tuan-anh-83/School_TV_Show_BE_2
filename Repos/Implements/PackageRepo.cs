using BOs.Models;
using DAOs;
using Repos.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos.Implements
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

        public async Task<List<Package>> GetAllActivePackagesAsync()
        {
            return await PackageDAO.Instance.GetAllActivePackagesAsync();
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
            return await PackageDAO.Instance.GetAllPackagesAsync();
        }

        public async Task<Package?> GetPackageByIdAsync(int packageId)
        {
            return await PackageDAO.Instance.GetPackageByIdAsync(packageId);
        }

        public async Task<bool> UpdatePackageAsync(Package package)
        {
            return await PackageDAO.Instance.UpdatePackageAsync(package);
        }

        public async Task<List<object>> GetTopPurchasedPackagesAsync()
        {
            return await PackageDAO.Instance.GetTopPurchasedPackagesAsync();
        }

        public async Task<List<Package>> SearchPackagesByNameAsync(string name)
        {
            return await PackageDAO.Instance.SearchPackagesByNameAsync(name);
        }
        public async Task<IEnumerable<Order>> GetPendingOrdersOlderThanAsync(TimeSpan timeSpan)
        {
            return await OrderDAO.Instance.GetPendingOrdersOlderThanAsync(timeSpan);
        }
    }
}
