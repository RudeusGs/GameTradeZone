using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Interfaces
{
    public interface IForumsCategoryService
    {
       public Task<ForumsCategory> CreateCategory(ForumsCategoryModel model);
       public Task<List<ForumsCategory>> GetAllCategories();
       public Task<ApiResult> DeleteCategory(int Id);
       public Task<ApiResult> GetAllCategoryStat();
    }
}
