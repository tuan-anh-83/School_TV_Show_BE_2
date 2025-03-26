using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IProgramRepo
    {
        Task<IEnumerable<Program>> GetAllProgramsAsync();
        Task<Program?> GetProgramByIdAsync(int programId);
        Task<IEnumerable<Program>> SearchProgramsByNameAsync(string name);
        Task<Program> CreateProgramAsync(Program program);
        Task<bool> UpdateProgramAsync(Program program);
        Task<bool> DeleteProgramAsync(int programId);
        Task<int> CountProgramsAsync();
        Task<int> CountProgramsByStatusAsync(string status);
        Task<int> CountProgramsByScheduleAsync(int scheduleId);
    }
}
