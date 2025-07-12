using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Areas.Order.ViewModel;
using Microsoft.AspNetCore.Authorization;
using WebBanHang.Areas.Order.Services;

namespace WebBanHang.Areas.Order.Controllers;

[Area("Order")]
[Route("api/order")]
[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly UserManager<IdentityUser> _userManager;
    public OrderController(UserManager<IdentityUser> userManager, IOrderService orderService)
    {
        _userManager = userManager;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _orderService.GetOrders(pageNumber, limit, user.Id);

            return Ok(new
            {
                orders = result.orderVMs,
                totalOrders = result.totalOrders
            });
        }
        catch { return BadRequest("Lấy hóa đơn thất bại"); }
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            return Ok(await _orderService.GetOneOrder(id, user.Id));
        }
        catch { return BadRequest("Lấy hóa đơn thất bại"); }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] OrderVM orderVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                await _orderService.Create(orderVM, user.Id);

                return Ok("Tạo hóa đơn thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch
        { return BadRequest("Tạo hóa đơn thất bại"); }
    }

    [HttpPost("completed-order/{id}")]
    public async Task<IActionResult> CompletedOrder(int id, [FromBody] bool completed)
    {
        try
        {
            if (completed)
            {
                var user = await _userManager.GetUserAsync(User);
                await _orderService.CompletedOrder(id, completed, user.Id);
                return Ok("Hoàn thành đơn hàng thành công");
            }
            else
            {
                throw new Exception();
            }
        }
        catch
        {
            return BadRequest("Hoàn thành đơn thất bại");
        }
    }

    //Xóa hóa đơn
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            await _orderService.Delete(id, user.Id);
            return Ok("Xóa hóa đơn thành công");
        }
        catch { return BadRequest("Xóa hóa đơn thất bại"); }
    }

    //Hủy hóa đơn (xóa hóa đơn và hiệu lực của hóa đơn), hồi phục lại số sản phẩm
    [HttpPost("destroy/{id}")]
    public async Task<IActionResult> Destroy(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            await _orderService.Destroy(id, user.Id);
            return Ok("Hủy hóa đơn thành công");
        }
        catch { return BadRequest("Hủy hóa đơn thất bại"); }
    }
}