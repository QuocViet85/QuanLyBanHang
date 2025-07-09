using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.DynamicAttribute.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.DynamicAttribute.Controllers;

[Area("DynamicAttribute")]
[Route("api/dynamicattribute")]
[Authorize]
public class AttributeController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public AttributeController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            List<AttributeModel> attributes;

            if (pageNumber > 0 && limit > 0)
            {
                attributes = await _dbContext.Attributes.Where(a => a.UserId == user.Id)
                                                        .Skip((pageNumber - 1) * limit)
                                                        .Take(limit)
                                                        .ToListAsync();
            }
            else
            {
                attributes = await _dbContext.Attributes.Where(a => a.UserId == user.Id).ToListAsync();
            }

            int totalDynamicAttributes = await _dbContext.Attributes.CountAsync();

            List<AttributeVM> attributeVMs = new List<AttributeVM>();

            if (attributes?.Count > 0)
            {
                attributeVMs = attributes.Select(a => GetAttributeVMFromAttribute(a)).ToList();
            }
            return Ok(new
            {
                dynamicAttributes = attributeVMs,
                totalDynamicAttributes = totalDynamicAttributes
            });
        }
        catch { return BadRequest("Lấy thuộc tính động thất bại"); }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] AttributeVM attributeVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var attributeModel = new AttributeModel()
                {
                    Name = attributeVM.Name,
                    UserId = user.Id
                };

                await _dbContext.Attributes.AddAsync(attributeModel);
                await _dbContext.SaveChangesAsync();

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
    public async Task<IActionResult> Update(int id, [FromBody] AttributeVM AttributeVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var attributeUpdate = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstOrDefaultAsync();

                if (attributeUpdate != null)
                {
                    attributeUpdate.Name = AttributeVM.Name;

                    _dbContext.Attributes.Update(attributeUpdate);
                    int result = await _dbContext.SaveChangesAsync();

                    return Ok("Cập nhật thuộc tính động thành công");
                }
                else
                {
                    throw new Exception();
                }
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
            var attributeDelete = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstOrDefaultAsync();
            if (attributeDelete != null)
            {
                _dbContext.Attributes.Remove(attributeDelete);
                int result = await _dbContext.SaveChangesAsync();
                return Ok("Xóa thuộc tính động thành công");
            }
            else
            {
                throw new Exception();
            }
        }
        catch { return BadRequest("Xóa thuộc tính động thất bại"); }
    }

    private AttributeVM GetAttributeVMFromAttribute(AttributeModel attribute)
    {
        return new AttributeVM()
        {
            Id = attribute.Id,
            Name = attribute.Name
        };
    }
}