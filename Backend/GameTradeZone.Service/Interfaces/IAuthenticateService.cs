using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Authenticate;
using Microsoft.AspNetCore.Identity;

namespace GameTradeZone.Service.Interfaces
{
    public interface IAuthenticateService
    {
        Task<ApiResult> Register(RegisterModel model);
        Task<ApiResult> Login(LoginModel model);
        Task<ApiResult> GetUserRoles(int userId);
        Task<ApiResult> ExternalLoginAsync(ExternalLoginInfo info);
        Task<ApiResult> UpdateUser(string userId, UpdateUserModel model);
        Task<ApiResult> SendOtpForEmailVerificationAsync(string email);
        Task<ApiResult> VerifyOtpForEmailVerificationAsync(VerifyOtpModel model);
    } 
}
