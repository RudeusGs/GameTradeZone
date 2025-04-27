using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.AccountGame;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountGameController : BaseController
    {
        private readonly IAccountGameService _accountGameService;

        public AccountGameController(IAccountGameService accountGameService)
        {
            _accountGameService = accountGameService;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _accountGameService.GetAll();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-All-By-UserID")]
        public async Task<IActionResult> GetAllByUserID(int userId)
        {
            try
            {
                var result = await _accountGameService.GetAllByUserID(userId);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromForm]AddAccountGameModel model)
        {
            try
            {
                var result = await _accountGameService.Add(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Update")]
        public async Task<IActionResult> Update(UpdateAccountGameModel model)
        {
            try
            {
                var result = await _accountGameService.Update(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _accountGameService.Delete(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Buy")]
        public async Task<IActionResult> Buy(BuyAccountGameModel model)
        {
            try
            {
                var result = await _accountGameService.Buy(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("AccountCheck")]
        public async Task<IActionResult> AccountCheck(int id)
        {
            try
            {
                var result = await _accountGameService.CheckAccount(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-Infor-User")]
        public async Task<IActionResult> GetInforUser(int id)
        {
            try
            {
                var result = await _accountGameService.GetInforUser(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-All-User-Data-Stat")]
        public async Task<IActionResult> GetAllUserDataStat()
        {
            try
            {
                var result = await _accountGameService.GetAllUserDataStat();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [HttpGet("Get-All-Paged")]
        public async Task<IActionResult> GetAllPaged(int pageIndex = 1, int pageSize = 8)
        {
            try
            {
                var result = await _accountGameService.GetAllPaged(pageIndex, pageSize);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
