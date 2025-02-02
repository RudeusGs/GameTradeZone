using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class RechargeBankController : ControllerBase
{
    private readonly IRechargeBankService _rechargeBankService;

    public RechargeBankController(IRechargeBankService rechargeBankService)
    {
        _rechargeBankService = rechargeBankService;
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetAllTransactions()
    {
        var result = await _rechargeBankService.GetAllRechargeBankTransactions();
        return result.Data != null ? Ok(result.Data) : BadRequest(result.Message);
    }
}
