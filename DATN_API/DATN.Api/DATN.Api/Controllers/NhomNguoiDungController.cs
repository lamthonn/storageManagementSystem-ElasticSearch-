using DATN.Application.DanhMucCapDo;
using DATN.Application.Interfaces;
using DATN.Application.Utils;
using DATN.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Api.Controllers
{
    [Route("api/nhom-nguoi-dung")]
    [ApiController]
    public class NhomNguoiDungController : ControllerBase
    {
        private readonly INhomNguoiDung _service;
        public NhomNguoiDungController(INhomNguoiDung service)
        {
            _service = service;
        }

        // GET: api/DanhMucCapDo/get-all
        [HttpGet("get-all")]
        [Authorize]
        public async Task<ActionResult<PaginatedList<nhom_nguoi_dung_dto>>> GetAll([FromQuery] nhom_nguoi_dung_dto request)
        {
            try
            {
                var result = await _service.GetPaginNhomNguoiDung(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("phan-quyen")]
        [Authorize]
        public async Task<ActionResult<string>> PhanQuyen([FromBody] PhanQuyenDto request)
        {
            try
            {
                var result = await _service.PhanQuyen(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
