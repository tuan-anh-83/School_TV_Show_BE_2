using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class ReportRepo : IReportRepo
    {
        public async Task<Report> CreateReportAsync(Report report)
        {
            return await ReportDAO.Instance.CreateReportAsync(report);
        }

        public async Task<Report?> GetReportByIdAsync(int reportId)
        {
            return await ReportDAO.Instance.GetReportByIdAsync(reportId);
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await ReportDAO.Instance.GetAllReportsAsync();
        }

        public async Task<bool> UpdateReportAsync(Report report)
        {
            return await ReportDAO.Instance.UpdateReportAsync(report);
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            return await ReportDAO.Instance.DeleteReportAsync(reportId);
        }

        public async Task<int> GetTotalReportsAsync()
        {
            return await ReportDAO.Instance.GetTotalReportsAsync();
        }

        public async Task<Dictionary<int, int>> GetReportsPerVideoAsync()
        {
            return await ReportDAO.Instance.GetReportsPerVideoAsync();
        }
    }
}
