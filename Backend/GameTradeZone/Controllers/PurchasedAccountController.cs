using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.PurchasedAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasedAccountController : BaseController
    {
        private readonly IPurchasedAccountService _purchasedAccountService;

        public PurchasedAccountController(IPurchasedAccountService purchasedAccountService)
        {
            _purchasedAccountService = purchasedAccountService;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _purchasedAccountService.GetAll();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-All-By-User-Id")]
        public async Task<IActionResult> GetAllByUserID(int id)
        {
            try
            {
                var result = await _purchasedAccountService.GetAllByUserID(id);
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
                var result = await _purchasedAccountService.Delete(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Confirm-Account")]
        public async Task<IActionResult> ConfirmAccount(ComfirmModel model)
        {
            try
            {
                var result = await _purchasedAccountService.ComfirmAccount(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}