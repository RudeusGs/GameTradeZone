using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.AccountGame;

namespace GameTradeZone.Service.Services
{
    public class AccountGameService : ServiceBase, IAccountGameService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        public AccountGameService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService,FileUploadService fileUploadService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
        }

        public Task<ApiResult> Add(AddAccountGameModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> BargainPrice(BargainAccountGameModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> Buy(BuyAccountGameModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> Delete(int id)
        {
            throw new NotImplementedException();
        }


        public Task<ApiResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetAllByUserID(int UserId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetByIdByAdmin(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetInforUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> Update(UpdateAccountGameModel model)
        {
            throw new NotImplementedException();
        }
    }
}
