using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Post;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Services
{
    public class ForumsService : IForumsCategoryService
    {
        private readonly DataContext _context;
        public ForumsService(DataContext context)
        {
            _context = context;
        }
        public async Task<ForumsCategory> CreateCategory(ForumsCategoryModel model)
        {
            var category = new ForumsCategory
            {
                
                Name = model.Name,
                Description = model.Description,
                IconClass = model.IconClass
            };
            await _context.ForumsCategories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<ApiResult> DeleteCategory(int Id)
        {
            var category = await _context.ForumsCategories.FindAsync(Id);
            if (category == null)
            {
                return new ApiResult
                {
                    Message = "Category not found"
                };
            }
            _context.ForumsCategories.Remove(category);
            await _context.SaveChangesAsync();
            return new ApiResult
            {
                Message = "Category deleted"
            };
        }

        public async Task<List<ForumsCategory>> GetAllCategories()
        {
            return await _context.ForumsCategories.ToListAsync();
        }

        public async Task<ApiResult> GetAllCategoryStat()
        {
            var categoryStats = await _context.ForumsCategories
          .Select(category => new
          {
              CategoryId = category.Id,
              CategoryName = category.Name,
              PostCount = category.PostCount
          })
          .ToListAsync();

            return new ApiResult(categoryStats);
        }
    }
}
