using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GameTradeZone.Service.Configurations
{
    public static class ApplicationServiceCollection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Common services
            // JWT
            services.AddScoped<IJwtService, JwtService>();

            // User Management Service.
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>, SignInManager<User>>();
            services.AddScoped<Common.IServices.IUserService, UserService>();

            // Ftp
            services.AddScoped<IFtpDirectoryService, FtpDirectoryService>();
            #endregion 
            services.AddHttpContextAccessor();
            #region Business services
            #endregion
            return services;
        }
    }
}
