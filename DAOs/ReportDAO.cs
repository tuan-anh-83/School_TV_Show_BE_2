using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class ReportDAO
    {
        private static ReportDAO instance = null;
        private readonly DataContext _context;

        private ReportDAO()
        {
            _context = new DataContext();
        }

        public static ReportDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ReportDAO();
                }
                return instance;
            }
        }

        public async Task<Report> CreateReportAsync(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            bool accountExists = await _context.Accounts.AnyAsync(a => a.AccountID == report.AccountID);
            bool videoExists = await _context.VideoHistories.AnyAsync(v => v.VideoHistoryID == report.VideoHistoryID);

            if (!accountExists || !videoExists)
                throw new Exception("Invalid AccountID or VideoHistoryID.");

            report.CreatedAt = DateTime.UtcNow;
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            if (reportId <= 0)
                throw new ArgumentException("Report ID must be greater than zero.");

            return await _context.Reports
                .Include(r => r.Account)
                .Include(r => r.VideoHistory)
                .FirstOrDefaultAsync(r => r.ReportID == reportId);
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.Account)
                .Include(r => r.VideoHistory)
                .ToListAsync();
        }

        public async Task<bool> UpdateReportAsync(Report report)
        {
            if (report == null || report.ReportID <= 0)
                throw new ArgumentException("Invalid Report data.");

            var existingReport = await _context.Reports.FindAsync(report.ReportID);
            if (existingReport == null)
                return false;

            existingReport.Reason = report.Reason;

            _context.Reports.Update(existingReport);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            if (reportId <= 0)
                throw new ArgumentException("Report ID must be greater than zero.");

            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return false;

            _context.Reports.Remove(report);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetTotalReportsAsync()
        {
            return await _context.Reports.CountAsync();
        }

        public async Task<Dictionary<int, int>> GetReportsPerVideoAsync()
        {
            return await _context.Reports
                .GroupBy(r => r.VideoHistoryID)
                .Select(g => new { VideoHistoryID = g.Key, TotalReports = g.Count() })
                .ToDictionaryAsync(x => x.VideoHistoryID, x => x.TotalReports);
        }
    }
}
