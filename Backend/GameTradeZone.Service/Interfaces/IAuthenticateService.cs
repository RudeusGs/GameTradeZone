using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Authenticate;

namespace GameTradeZone.Service.Interfaces
{
    public interface IAuthenticateService
    {
        Task<ApiResult> Register(RegisterModel model);
        Task<ApiResult> Login(LoginModel model);
        Task<ApiResult> GetUserRoles(int userId);

    }
}
