using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SchoolChannelService : ISchoolChannelService
    {
        private readonly ISchoolChannelRepo _SchoolChannelRepo;
        public SchoolChannelService(ISchoolChannelRepo schoolChannelRepo)
        {
            _SchoolChannelRepo = schoolChannelRepo;
        }

        public async Task<IEnumerable<SchoolChannel>> GetAllAsync()
        {
            return await _SchoolChannelRepo.GetAllAsync();
        }

        public async Task<IEnumerable<SchoolChannel>> GetAllActiveAsync()
        {
            return await _SchoolChannelRepo.GetAllActiveAsync();
        }

        public async Task CreateAsync(SchoolChannel schoolChannel)
        {
            await _SchoolChannelRepo.AddAsync(schoolChannel);
        }

        public async Task UpdateAsync(SchoolChannel schoolChannel)
        {
            await _SchoolChannelRepo.UpdateAsync(schoolChannel);
        }

        public async Task<bool> DeleteByNameAsync(string name)
        {
            return await _SchoolChannelRepo.DeleteByNameAsync(name);
        }

        public async Task<SchoolChannel> GetByIdAsync(int id)
        {
            var result = await _SchoolChannelRepo.GetByIdAsync(id);
            return result!;
        }

        public async Task<IEnumerable<SchoolChannel>> SearchAsync(string? keyword, string? address, int? accountId)
        {
            return await _SchoolChannelRepo.SearchAsync(keyword, address, accountId);
        }
        public async Task<bool> SchoolChannelExistsAsync(int schoolChannelId)
        {
            var schoolChannel = await _SchoolChannelRepo.GetByIdAsync(schoolChannelId);
            return schoolChannel != null;
        }

        public Task<IEnumerable<object>> GetAllChannelsAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<bool> DoesAccountHaveSchoolChannelAsync(int accountId)
        {
            return await _SchoolChannelRepo.DoesAccountHaveSchoolChannelAsync(accountId);
        }
    }
}
