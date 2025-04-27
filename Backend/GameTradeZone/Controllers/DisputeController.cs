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
        private readonly IDisputeService _disputeService;

        public DisputeController(IDisputeService disputeService)
        {
            _disputeService = disputeService;
        }

        /// <summary>
        /// Thêm một tranh chấp mới.
        /// </summary>
        /// <param name="model">Thông tin tranh chấp.</param>
        /// <returns>Kết quả thêm tranh chấp.</returns>
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

        /// <summary>
        /// Xóa một tranh chấp theo ID.
        /// </summary>
        /// <param name="id">ID của tranh chấp.</param>
        /// <returns>Kết quả xóa tranh chấp.</returns>
        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _disputeService.Delete(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        /// <summary>
        /// Lấy tất cả các tranh chấp.
        /// </summary>
        /// <returns>Danh sách tất cả các tranh chấp.</returns>
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _disputeService.GetAll();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        /// <summary>
        /// Lấy tất cả các tranh chấp theo ID của người bán.
        /// </summary>
        /// <param name="id">ID của người bán.</param>
        /// <returns>Danh sách các tranh chấp của người bán.</returns>
        [Authorize]
        [HttpGet("GetAllBySellerId")]
        public async Task<IActionResult> GetAllBySellerId(int id)
        {
            try
            {
                var result = await _disputeService.GetAllBySellerId(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        /// <summary>
        /// Lấy tất cả các tranh chấp theo ID của người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng.</param>
        /// <returns>Danh sách các tranh chấp của người dùng.</returns>
        [Authorize]
        [HttpGet("GetAllByUserId")]
        public async Task<IActionResult> GetAllByUserId(int id)
        {
            try
            {
                var result = await _disputeService.GetAllByUserId(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        /// <summary>
        /// Lấy thông tin tranh chấp theo ID.
        /// </summary>
        /// <param name="id">ID của tranh chấp.</param>
        /// <returns>Thông tin chi tiết của tranh chấp.</returns>
        [Authorize]
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _disputeService.GetById(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        /// <summary>
        /// Phản hồi một tranh chấp.
        /// </summary>
        /// <param name="id">ID của tranh chấp.</param>
        /// <param name="rep">Nội dung phản hồi.</param>
        /// <returns>Kết quả phản hồi tranh chấp.</returns>
        [Authorize]
        [HttpPut("Reply")]
        public async Task<IActionResult> Reply(int id, [FromBody] string rep)
        {
            try
            {
                var result = await _disputeService.Reply(id, rep);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        /// <summary>
        /// Cập nhật thông tin tranh chấp.
        /// </summary>
        /// <param name="model">Thông tin cập nhật tranh chấp.</param>
        /// <returns>Kết quả cập nhật tranh chấp.</returns>
        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateDisputeModel model)
        {
            try
            {
                var result = await _disputeService.Update(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}