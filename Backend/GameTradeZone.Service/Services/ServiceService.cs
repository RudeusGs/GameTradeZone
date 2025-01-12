using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Dispute;

namespace GameTradeZone.Service.Services
{
    public class ServiceService : ServiceBase, IServiceService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        public ServiceService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService, FileUploadService fileUploadService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
        }

        public Task<ApiResult> Add(AddDisputeModel model)
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

        public Task<ApiResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> Update(UpdateDisputeModel model)
        {
            throw new NotImplementedException();
        }
    }
}
