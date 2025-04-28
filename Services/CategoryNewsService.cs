using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repos;

namespace Services
{
    public class CategoryNewsService : ICategoryNewsService
    {
        private readonly ICategoryNewsRepo _categoryNewsRepository;

        public CategoryNewsService(ICategoryNewsRepo categoryNewsRepository)
        {
            _categoryNewsRepository = categoryNewsRepository;
        }

        public async Task<IEnumerable<CategoryNews>> GetAllAsync()
        {
            return await _categoryNewsRepository.GetAllAsync();
        }

        public async Task<CategoryNews?> GetByIdAsync(int id)
        {
            return await _categoryNewsRepository.GetByIdAsync(id);
        }

        public async Task<CategoryNews> AddAsync(CategoryNews categoryNews)
        {
            if (categoryNews == null)
                throw new ArgumentNullException(nameof(categoryNews));


            categoryNews.CategoryNewsID = 0;

            return await _categoryNewsRepository.AddAsync(categoryNews);
        }

        public async Task<bool> UpdateAsync(CategoryNews categoryNews)
        {
            return await _categoryNewsRepository.UpdateAsync(categoryNews);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _categoryNewsRepository.DeleteAsync(id);
        }
    }
}
