using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class WebsiteAccountService : ServiceBase, IWebsiteAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly CloudinaryService _cloudinaryService;

        public WebsiteAccountService(
            DataContext dataContext,
            IUserService userService,
            CloudinaryService cloudinaryService,
            UserManager<User> userManager)
            : base(dataContext, userService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResult> BlockAccount(int id)
        {
            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return new ApiResult { Message = "Người dùng không tồn tại" };
                }

                user.Status = true;
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();

                return new ApiResult();
            }
            catch (Exception ex)
            {
                return new ApiResult { Message = $"Error: {ex.Message} " };
            }
        }

        public async Task<ApiResult> GetAll()
        {
            var result = await _dataContext.Users.ToListAsync();
            return new ApiResult(result);
        }

        public async Task<ApiResult> GetById(int id)
        {
            var result = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            return new ApiResult(result);
        }

        public async Task<ApiResult> UpdateRoles(int id, List<string> newRoles)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return new ApiResult { Message = "Người dùng không tồn tại!" };
                }
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(newRoles).ToList();
                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        return new ApiResult();
                    }
                }
                var rolesToAdd = newRoles.Except(currentRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        return new ApiResult();
                    }
                }

                user.UpdatedDate = DateTime.UtcNow;
                await _dataContext.SaveChangesAsync();

                return new ApiResult();
            }
            catch (Exception ex)
            {
                return new ApiResult { Message = $"Error: {ex.Message} " };
            }
        } 
    public async Task<ApiResult> UpdateUserAvatar(int id, IFormFile? image)
        {
            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return new ApiResult { Message = "Người dùng không tồn tại!" };
                }
                string imageUrl = null;
                if (image != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(image);
                }
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                return new ApiResult { Message = $"Error: {ex.Message} " };
            }
        }
    }
}