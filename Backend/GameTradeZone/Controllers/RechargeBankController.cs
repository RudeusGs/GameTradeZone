using GameTradeZone.Controllers;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.RechargeBank;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class RechargeBankController : BaseController
{
    private readonly IRechargeBankService _rechargeBankService;

    public RechargeBankController(IRechargeBankService rechargeBankService)
    {
        _rechargeBankService = rechargeBankService;
    }
    [Authorize]
    [HttpGet("transactions")]
    public async Task<IActionResult> GetAllTransactions()
    {
        try
        {
            var result = await _rechargeBankService.GetAllRechargeBankTransactions();
            return Response(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("withdraw")]
    public async Task<IActionResult> WithdrawMoneyRequest([FromBody] WithdrawMoneyModel model)
    {
        try
        {
            var result = await _rechargeBankService.WithdrawMoneyRequest(model);
            return Response(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("withdraw/{id}")]
    public async Task<IActionResult> WithdrawMoneyConfirmRequest(int id)
    {
        try
        {
            var result = await _rechargeBankService.WithdrawMoneyConfirmRequest(id);
            return Response(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [Authorize]
    [HttpGet("withdraws")]
    public async Task<IActionResult> GetAllWithDrawTransactionsByUser()
    {
        try
        {
            var result = await _rechargeBankService.GetAllWithDrawTransactionsByUser();
            return Response(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [Authorize]
    [HttpGet("recharge")]
    public async Task<IActionResult> GetAllRechargeBankTransactionsByUser()
    {
        try
        {
            var result = await _rechargeBankService.GetAllRechargeBankTransactionsByUser();
            return Response(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    [HttpGet("all-withdraws")]
    public async Task<IActionResult> GetAllWithdrawMoneyRequest()
    {
        try
        {
            var result = await _rechargeBankService.GetAllWithdrawMoneyRequest();
            return Response(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
