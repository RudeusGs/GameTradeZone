using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameAccountField;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class GameAccountFieldService : ServiceBase, IGameAccountFieldService
    {
        public GameAccountFieldService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> Add(AddGameAccountFieldModel model)
        {
            var result = await _dataContext.GameAccountFields.FirstOrDefaultAsync(x => x.GameFieldId == model.GameFieldId && x.GameAccountId == model.GameAccountId);
            var tran = await _dataContext.Database.BeginTransactionAsync();
            if (result == null)
            {
                var newGameAccountField = new GameAccountField
                {
                    GameAccountId = model.GameAccountId,
                    GameFieldId = model.GameFieldId,
                    FieldValue = model.FieldValue,
                    CreatedDate = DateTime.Now,
                };
                _dataContext.GameAccountFields.Add(newGameAccountField);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult(result);
            }
            return new ApiResult { Message = "Lỗi" };
        }

        public async Task<ApiResult> GetByID(int GameAcountId, int GameFieldId)
        {
            var result = await _dataContext.GameAccountFields.Where(x => x.GameAccountId == GameAcountId && x.GameFieldId == GameFieldId).ToListAsync();
            return new(result);
        }
    }
}
