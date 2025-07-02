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

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var attributes = await _dbContext.Attributes.Where(a => a.UserId == user.Id).ToListAsync();

        return View(attributes);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var attribute = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstAsync();

        return View(attribute);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(AttributeVM attributeVM)
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

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var attributeUpdate = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstOrDefaultAsync();

        var AttributeVMUpdate = new AttributeVM()
        {
            Name = attributeUpdate.Name,
        };

        return View(AttributeVMUpdate);
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, AttributeVM AttributeVM)
    {
        var user = await _userManager.GetUserAsync(User);

        try
        {
            var attributeUpdate = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstAsync();

            if (attributeUpdate != null)
            {
                attributeUpdate.Name = AttributeVM.Name;

                _dbContext.Attributes.Update(attributeUpdate);
                int result = await _dbContext.SaveChangesAsync();

                return RedirectToAction("Update", new { id = id });
            }
            return Redirect("Index");
        }
        catch
        {
            return Redirect("Index");
        }
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        try
        {
            var attributeDelete = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == user.Id).FirstAsync();
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