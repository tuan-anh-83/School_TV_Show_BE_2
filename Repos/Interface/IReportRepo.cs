using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos.Interface
{
    public interface IReportRepo
    {
        Task<Report> CreateReportAsync(Report report);
        Task<Report?> GetReportByIdAsync(int reportId);
        Task<IEnumerable<Report>> GetAllReportsAsync();
        Task<bool> UpdateReportAsync(Report report);
        Task<bool> DeleteReportAsync(int reportId);
        Task<int> GetTotalReportsAsync();
        Task<Dictionary<int, int>> GetReportsPerVideoAsync();
    }
}
