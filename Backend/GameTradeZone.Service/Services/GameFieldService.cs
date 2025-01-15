using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameField;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class GameFieldService : ServiceBase, IGameFieldService
    {
        public GameFieldService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> Add(AddGameFieldModel model)
        {
            var gameField = await _dataContext.GameFields.AsNoTracking().FirstOrDefaultAsync(x => x.FieldName == model.FieldName && x.GameInforID == model.GameInforID);
            if(gameField != null && gameField.IsDelete == false) 
            {
                return new ApiResult { Message = "Thuộc tính này đã tồn tại! Vui lòng thêm thuộc tính khác" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newGameField = new GameField
                {
                    GameInforID = model.GameInforID,
                    FieldName = model.FieldName,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };

                _dataContext.GameFields.Add(newGameField);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();

                return new ApiResult(newGameField);
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> Delete(int id)
        {
            var gameField = await _dataContext.GameFields.FirstOrDefaultAsync(x => x.Id == id);
            if(gameField == null)
            {
                return new ApiResult { Message = "Thuộc tính này không tồn tại" };
            }
            if(gameField.IsDelete == true)
            {
                return new ApiResult { Message = "Thuộc tính này đã xóa rồi! Không thể xóa nữa" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                gameField.IsDelete = true;
                gameField.DeleteDate = DateTime.Now;

                _dataContext.Update(gameField);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Xóa thành công" };
            }
            catch(Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> GetAll()
        {
            var gameField = await _dataContext.GameFields.Where(x => x.IsDelete == false).ToListAsync();
            return new(gameField);
        }

        public async Task<ApiResult> GetById(int id)
        {
            var gameField = await _dataContext.GameFields.FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == false);
            return new(gameField);
        }

        public async Task<ApiResult> Update(UpdateGameFieldModel model)
        {
            var gameField = await _dataContext.GameFields.FirstOrDefaultAsync(x => x.Id ==  model.Id);
            if(gameField == null)
            {
                return new ApiResult { Message = "Không tìm thấy thuộc tính" };
            }
            if(gameField.IsDelete == true)
            {
                return new ApiResult { Message = "Thuộc tính này đã bị xóa" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                gameField.GameInforID = model.GameInforID ?? gameField.GameInforID;
                gameField.FieldName = model.FieldName ?? gameField.FieldName;
                gameField.UpdatedDate = DateTime.Now;

                _dataContext.GameFields.Update(gameField);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();

                return new ApiResult { Message = "Cập nhật thành công!" };
            }
            catch(Exception e) 
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }
    }
}
