using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repos;

namespace Services
{
    public class ProgramService : IProgramService
    {
        private readonly IProgramRepo _programRepository;

        public ProgramService(IProgramRepo programRepository)
        {
            _programRepository = programRepository;
        }

        public async Task<Program> CreateProgramAsync(Program program)
        {
            if (program == null || string.IsNullOrWhiteSpace(program.ProgramName))
                throw new Exception("Program must have a valid name.");

            return await _programRepository.CreateProgramAsync(program);
        }

        public async Task<Program?> GetProgramByIdAsync(int programId)
        {
            if (programId <= 0)
                throw new Exception("Invalid Program ID.");

            var program = await _programRepository.GetProgramByIdAsync(programId);
            if (program == null)
                throw new Exception("Program not found.");

            return program;
        }

        public async Task<IEnumerable<Program>> GetAllProgramsAsync()
        {
            var programs = await _programRepository.GetAllProgramsAsync();
            if (!programs.Any())
                throw new Exception("No programs found.");

            return programs;
        }

        public async Task<bool> UpdateProgramAsync(Program program)
        {
            if (program == null || program.ProgramID <= 0)
                throw new Exception("Invalid Program.");

            return await _programRepository.UpdateProgramAsync(program);
        }

        public async Task<bool> DeleteProgramAsync(int programId)
        {
            if (programId <= 0)
                throw new Exception("Invalid Program ID.");

            return await _programRepository.DeleteProgramAsync(programId);
        }

        public async Task<IEnumerable<Program>> SearchProgramsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Program name cannot be null or empty.");

            var programs = await _programRepository.SearchProgramsByNameAsync(name);
            if (!programs.Any())
                throw new Exception($"No programs found with name: {name}.");

            return programs;
        }

        public async Task<int> CountProgramsAsync()
        {
            return await _programRepository.CountProgramsAsync();
        }

        public async Task<int> CountProgramsByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new Exception("Status cannot be null or empty.");

            return await _programRepository.CountProgramsByStatusAsync(status);
        }

        public async Task<int> CountProgramsByScheduleAsync(int scheduleId)
        {
            if (scheduleId <= 0)
                throw new Exception("Invalid Schedule ID.");

            return await _programRepository.CountProgramsByScheduleAsync(scheduleId);
        }
    }
}
