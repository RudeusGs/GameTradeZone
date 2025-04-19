using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.Post;
using Microsoft.AspNetCore.Mvc;

namespace GameTradeZone.Controllers
{
    [Route("api/forumscategory")]
    [ApiController]
    public class ForumsCategoryController : BaseController
    {
        private readonly IForumsCategoryService _forumsCategoryService;
        public ForumsCategoryController(IForumsCategoryService forumsCategoryService)
        {
            _forumsCategoryService = forumsCategoryService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory(ForumsCategoryModel request)
        {
            try
            {
                var category = await _forumsCategoryService.CreateCategory(request);
                return Response(category);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _forumsCategoryService.GetAllCategories();
                return Response(categories);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            try
            {
                var result = await _forumsCategoryService.DeleteCategory(Id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [HttpGet("getallcategorypostcount")]
        public async Task<IActionResult> GetAllCategoryPostCount()
        {
            try
            {
                var result = await _forumsCategoryService.GetAllCategoryPostCount();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
