using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.Dispute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisputeController : BaseController
    {
        public IDisputeService _disputeService;
        public DisputeController(IDisputeService disputeService)
        {
            _disputeService = disputeService;
        }

        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddDisputeModel model)
        {
            try
            {
                var result = await _disputeService.Add(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

    }
}
