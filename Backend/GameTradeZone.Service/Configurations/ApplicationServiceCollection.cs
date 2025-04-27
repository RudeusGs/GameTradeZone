using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Common.Services;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Services;
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
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IGameInforService, GameInforService>();
            services.AddScoped<IGameFieldService, GameFieldService>();
            services.AddScoped<IPurchasedAccountService, PurchasedAccountService>();
            services.AddScoped<IOnGoingServiceService, OnGoingServiceService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILevelIconService, LevelIconService>();
            services.AddScoped<IHiredServiceService, HiredServiceService>();
            services.AddScoped<IDisputeService, DisputeService>();
            services.AddScoped<IAccountGameService, AccountGameService>();
            services.AddScoped<IWebsiteAccountService, WebsiteAccountService>();
            services.AddScoped<IGameAccountFieldService, GameAccountFieldService>();
            services.AddScoped<IAuctionService, AuctionService>();
            services.AddScoped<IChatService, ChatService>();
            #endregion
            return services;
        }
    }
}
