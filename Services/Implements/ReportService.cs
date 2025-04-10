using BOs.Models;
using Repos.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class ReportService : IReportService
    {
        private readonly IReportRepo _reportRepo;

        public ReportService(IReportRepo reportRepo)
        {
            _reportRepo = reportRepo;
        }

        public async Task<Report> CreateReportAsync(Report report)
        {
            return await _reportRepo.CreateReportAsync(report);
        }

        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            return await _reportRepo.GetReportByIdAsync(reportId);
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _reportRepo.GetAllReportsAsync();
        }

        public async Task<bool> UpdateReportAsync(Report report)
        {
            return await _reportRepo.UpdateReportAsync(report);
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            return await _reportRepo.DeleteReportAsync(reportId);
        }

        public async Task<int> GetTotalReportsAsync()
        {
            return await _reportRepo.GetTotalReportsAsync();
        }

        public async Task<Dictionary<int, int>> GetReportsPerVideoAsync()
        {
            return await _reportRepo.GetReportsPerVideoAsync();
        }
    }
}
