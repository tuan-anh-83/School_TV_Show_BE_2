﻿using BOs.Data;
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
            return await context.Packages
                .Where(p => p.Status == "Active")
                .ToListAsync();
        }

        public async Task<List<Package>> GetAllActivePackagesAsync()
        {
            return await context.Packages.ToListAsync();
        }

        public async Task<Package?> GetPackageByIdAsync(int packageId)
        {
            return await context.Packages
                .FirstOrDefaultAsync(p => p.PackageID == packageId && p.Status == "Active");
        }
        public async Task<List<Package>> SearchPackagesByNameAsync(string name)
        {
            return await context.Packages
                .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
                .ToListAsync();
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
        public async Task<List<object>> GetTopPurchasedPackagesAsync()
        {
            var result = await context.OrderDetails
                .GroupBy(od => od.PackageID)
                .Select(g => new
                {
                    PackageID = g.Key,
                    PurchaseCount = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.PurchaseCount)
                .ToListAsync();

            var packageDetails = await context.Packages
                .Where(p => result.Select(r => r.PackageID).Contains(p.PackageID))
                .ToListAsync();

            var rankedPackages = result.Select(r => new
            {
                PackageID = r.PackageID,
                PackageName = packageDetails.FirstOrDefault(p => p.PackageID == r.PackageID)?.Name ?? "Unknown",
                PurchaseCount = r.PurchaseCount
            }).ToList();

            return rankedPackages.Cast<object>().ToList();
        }
        public async Task<(Package?, int?)?> GetCurrentPackageAndDurationByAccountIdAsync(int accountId)
        {
            Console.WriteLine($"[DEBUG] Start fetching current package for AccountID: {accountId}");

            var latestPaidOrder = await context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Package)
                .Where(o => o.AccountID == accountId && o.Status == "Completed")
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestPaidOrder == null)
            {
                Console.WriteLine("[DEBUG] No completed order found for account.");
                return null;
            }

            Console.WriteLine($"[DEBUG] Found completed order: OrderID = {latestPaidOrder.OrderID}, CreatedAt = {latestPaidOrder.CreatedAt}");

            var packageDetail = latestPaidOrder.OrderDetails.FirstOrDefault();
            if (packageDetail == null)
            {
                Console.WriteLine("[DEBUG] Order found, but no order detail available.");
                return null;
            }

            Console.WriteLine($"[DEBUG] Found OrderDetail: PackageID = {packageDetail.PackageID}");

            if (packageDetail.Package == null)
            {
                Console.WriteLine("[DEBUG] Package in OrderDetail is null.");
                return null;
            }

            Console.WriteLine($"[DEBUG] Package loaded: Name = {packageDetail.Package.Name}, Duration = {packageDetail.Package.Duration}");

            var schoolChannel = await context.SchoolChannels
                .FirstOrDefaultAsync(sc => sc.AccountID == accountId);

            if (schoolChannel == null)
            {
                Console.WriteLine("[DEBUG] No SchoolChannel found for account.");
                return null;
            }

            Console.WriteLine($"[DEBUG] SchoolChannel found: TotalDuration = {schoolChannel.TotalDuration}");

            return (packageDetail.Package, schoolChannel.TotalDuration);
        }

    }
}
