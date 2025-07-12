using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Areas.DynamicAttribute.Services;
using WebBanHang.Areas.DynamicAttribute.ViewModel;

namespace WebBanHang.Areas.DynamicAttribute.Controllers;

[Area("DynamicAttribute")]
[Route("api/dynamicattribute")]
[Authorize]
public class DynamicAttributeController : Controller
{
    private readonly IDynamicAttributeService _dynamicAttributeService;
    private readonly UserManager<IdentityUser> _userManager;

    public DynamicAttributeController(IDynamicAttributeService dynamicAttributeService, UserManager<IdentityUser> userManager)
    {
        _dynamicAttributeService = dynamicAttributeService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _dynamicAttributeService.GetDynamicAttributes(pageNumber, limit, user.Id);

            return Ok(new
            {
                dynamicAttributes = result.dynamicAttributeVMs,
                totalDynamicAttributes = result.totalDynamicAttributes
            });
        }
        catch { return BadRequest("Lấy thuộc tính động thất bại"); }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] DynamicAttributeVM dynamicAttributeVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                await _dynamicAttributeService.Create(dynamicAttributeVM, user.Id);

                return Ok("Tạo thuộc tính động thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch { return BadRequest("Tạo thuộc tính động thất bại"); }
    }


    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DynamicAttributeVM dynamicAttributeVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                await _dynamicAttributeService.Update(id, dynamicAttributeVM, user.Id);

                return Ok("Cập nhật thuộc tính động thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }

        }
        catch { return BadRequest("Cập nhật thuộc tính động thất bại"); }
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            await _dynamicAttributeService.Delete(id, user.Id);
            
            return Ok("Xóa thuộc tính động thành công");
        }
        catch { return BadRequest("Xóa thuộc tính động thất bại"); }
    }
}