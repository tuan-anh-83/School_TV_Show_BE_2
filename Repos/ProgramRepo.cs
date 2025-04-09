using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class ProgramRepo : IProgramRepo
    {
        public async Task<IEnumerable<Program>> GetAllProgramsAsync()
        {
            return await ProgramDAO.Instance.GetAllProgramsAsync();
        }

        public async Task<Program?> GetProgramByIdAsync(int programId)
        {
            return await ProgramDAO.Instance.GetProgramByIdAsync(programId);
        }

        public async Task<IEnumerable<Program>> SearchProgramsByNameAsync(string name)
        {
            return await ProgramDAO.Instance.SearchProgramsByNameAsync(name);
        }

        public async Task<Program> CreateProgramAsync(Program program)
        {
            return await ProgramDAO.Instance.CreateProgramAsync(program);
        }

        public async Task<bool> UpdateProgramAsync(Program program)
        {
            return await ProgramDAO.Instance.UpdateProgramAsync(program);
        }

        public async Task<bool> DeleteProgramAsync(int programId)
        {
            return await ProgramDAO.Instance.DeleteProgramAsync(programId);
        }

        public async Task<int> CountProgramsAsync()
        {
            return await ProgramDAO.Instance.CountProgramsAsync();
        }

        public async Task<int> CountProgramsByStatusAsync(string status)
        {
            return await ProgramDAO.Instance.CountProgramsByStatusAsync(status);
        }

        public async Task<int> CountProgramsByScheduleAsync(int scheduleId)
        {
            return await ProgramDAO.Instance.CountProgramsByScheduleAsync(scheduleId);
        }

        public async Task<IEnumerable<Program>> GetProgramsByChannelIdAsync(int channelId)
        {
            return await ProgramDAO.Instance.GetProgramsByChannelIdAsync((int)channelId);   
        }
    }
}
