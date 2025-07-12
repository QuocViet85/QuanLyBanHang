using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Areas.Tax.Services;
using WebBanHang.Areas.Tax.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Tax.Controllers;

[Area("Tax")]
[Route("api/tax")]
public class TaxController : Controller
{
    private readonly ITaxService _taxService;
    private readonly UserManager<IdentityUser> _userManager;
    public TaxController(ITaxService taxService, UserManager<IdentityUser> userManager)
    {
        _taxService = taxService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _taxService.GetTaxes(pageNumber, limit, user.Id);

            return Ok(new
            {
                taxes = result.taxVMs,
                totalTaxes = result.taxVMs
            });
        }
        catch
        { return BadRequest("Lỗi lấy thuế"); }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetTaxActive()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var taxVMs = await _taxService.GetTaxActive(user.Id);
            return Ok(taxVMs);
        }
        catch
        { return BadRequest("Lấy thuế thất bại"); }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] TaxVM taxVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                await _taxService.Create(taxVM, user.Id);

                return Ok("Tạo thuế thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch { return BadRequest("Tạo thuế thất bại"); }

    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaxVM taxVM)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                await _taxService.Update(id, taxVM, user.Id);

                return Ok("Cập nhật thuế thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch { return BadRequest("Cập nhật thuế thất bại"); }

    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            await _taxService.Delete(id, user.Id);
            return Ok("Xóa thuế thành công");
        }
        catch { return BadRequest("Xóa thuế thất bại"); }
    }
}