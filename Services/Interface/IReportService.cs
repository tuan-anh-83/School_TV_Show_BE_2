using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IReportService
    {
        Task<Report> CreateReportAsync(Report report);
        Task<Report> GetReportByIdAsync(int reportId);
        Task<IEnumerable<Report>> GetAllReportsAsync();
        Task<bool> UpdateReportAsync(Report report);
        Task<bool> DeleteReportAsync(int reportId);

        Task<int> GetTotalReportsAsync();
        Task<Dictionary<int, int>> GetReportsPerVideoAsync();
    }
}
