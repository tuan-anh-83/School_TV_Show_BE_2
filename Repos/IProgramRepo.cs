using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IProgramRepo
    {
        Task<Program> CreateProgramAsync(Program program);
        Task<Program?> GetProgramByIdAsync(int programId);
        Task<IEnumerable<Program>> GetAllProgramsAsync();
        Task<bool> UpdateProgramAsync(Program program);
        Task<bool> DeleteProgramAsync(int programId);
        Task<IEnumerable<Program>> SearchProgramsByNameAsync(string name);
        Task<int> CountProgramsAsync();
        Task<int> CountProgramsByStatusAsync(string status);
        Task<int> CountProgramsByScheduleAsync(int scheduleId);
        Task<IEnumerable<Program>> GetProgramsByChannelIdAsync(int channelId);
        Task<List<Program>> GetProgramsWithVideosAsync();
        Task<List<Program>> GetProgramsWithoutVideosAsync();
        Task<List<Program>> GetProgramsByChannelIdWithIncludesAsync(int channelId);
    }
}
