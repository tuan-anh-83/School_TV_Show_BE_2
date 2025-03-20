using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class PackageDAO
    {
        private static PackageDAO instance = null;
        private readonly DataContext context;

        private PackageDAO()
        {
            context = new DataContext();
        }

        public static PackageDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PackageDAO();
                }
                return instance;
            }
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
            return await context.Packages.ToListAsync();
        }

        public async Task<List<Package>> GetActivePackagesAsync()
        {
            return await context.Packages
                .Where(p => p.Status == "Active")
                .ToListAsync();
        }

        public async Task<Package?> GetPackageByIdAsync(int packageId)
        {
            return await context.Packages.FirstOrDefaultAsync(p => p.PackageID == packageId);
        }

        public async Task<bool> AddPackageAsync(Package package)
        {
            package.Status = "Active";
            package.CreatedAt = DateTime.UtcNow;
            package.UpdatedAt = DateTime.UtcNow;

            await context.Packages.AddAsync(package);
            await context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> UpdatePackageAsync(Package package)
        {
            var existingPackage = await GetPackageByIdAsync(package.PackageID);
            if (existingPackage == null)
                return false;

            existingPackage.Name = package.Name;
            existingPackage.Description = package.Description;
            existingPackage.Price = package.Price;
            existingPackage.Duration = package.Duration;
            existingPackage.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeletePackageAsync(int packageId)
        {
            var package = await GetPackageByIdAsync(packageId);
            if (package == null)
                return false;

            package.Status = "Inactive";
            package.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
