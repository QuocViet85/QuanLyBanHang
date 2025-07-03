using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.DynamicAttribute.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.DynamicAttribute.Controllers;

[Area("DynamicAttribute")]
[Route("attribute")]
public class AttributeController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public AttributeController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    private AttributeVM GetAttributeVMFromAttribute(AttributeModel attribute)
    {
        return new AttributeVM()
        {
            Id = attribute.Id,
            Name = attribute.Name
        };
    }
    public async Task<IActionResult> Index()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var attributes = await _dbContext.Attributes.Where(a => a.UserId == user.Id).ToListAsync();

            List<AttributeVM> attributeVMs = new List<AttributeVM>();

            if (attributes.Count > 0)
            {
                attributeVMs = attributes.Select(a => GetAttributeVMFromAttribute(a)).ToList();
            }
            return View(attributeVMs);
        }
        catch { }
        return null;
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var attribute = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstOrDefaultAsync();

            if (attribute != null)
            {
                var attributeVM = GetAttributeVMFromAttribute(attribute);
                return View(attributeVM);
            }
        }
        catch { }
        return null;

    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(AttributeVM attributeVM)
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

                return RedirectToAction("Create");
            }
            return View();
        }
        catch { }
        return View();
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var attributeUpdate = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstOrDefaultAsync();

            if (attributeUpdate != null)
            {
                var AttributeVMUpdate = GetAttributeVMFromAttribute(attributeUpdate);

                return View(AttributeVMUpdate);
            }
        }
        catch { }

        return null;
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, AttributeVM AttributeVM)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var attributeUpdate = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstOrDefaultAsync();

            if (attributeUpdate != null)
            {
                attributeUpdate.Name = AttributeVM.Name;

                _dbContext.Attributes.Update(attributeUpdate);
                int result = await _dbContext.SaveChangesAsync();
            }
        }
        catch { }
        return RedirectToAction("Update", new { id = id });
    }

    [HttpGet("delete/{id}")]
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
            }
        }
        catch { }
        return RedirectToAction("Index");
    }
}