using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Tax.Model;
using WebBanHang.Areas.Tax.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Tax.Controllers;

[Area("Tax")]
[Route("api/tax")]
public class TaxController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    public TaxController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var taxes = await _dbContext.Taxes.Where(t => t.UserId == user.Id).ToListAsync();

            List<TaxViewModel> taxVMs = new List<TaxViewModel>();

            if (taxes.Count > 0)
            {
                taxVMs = taxes.Select(p => GetTaxVMFromtaxModel(p)).ToList();
            }
            return Ok(taxVMs);
        }
        catch
        { }
        return null;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetTaxActive()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var taxes = await _dbContext.Taxes.Where(t => t.UserId == user.Id && t.IsActive).ToListAsync();

            List<TaxViewModel> taxVMs = new List<TaxViewModel>();

            if (taxes.Count > 0)
            {
                taxVMs = taxes.Select(p => GetTaxVMFromtaxModel(p)).ToList();
            }
            return Ok(taxVMs);
        }
        catch
        { }
        return null;
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var tax = await _dbContext.Taxes.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

            if (tax != null)
            {
                var taxVM = GetTaxVMFromtaxModel(tax);
                return View(taxVM);
            }
        }
        catch { }

        return null;
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(TaxViewModel taxVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var taxModel = new TaxModel()
                {
                    Name = taxVM.Name,
                    Description = taxVM.Description,
                    Code = taxVM.Code,
                    IsActive = taxVM.IsActive,
                    IsDefault = taxVM.IsDefault,
                    UserId = user.Id,
                    Rate = taxVM.Rate,
                };

                await _dbContext.Taxes.AddAsync(taxModel);
                await _dbContext.SaveChangesAsync();
            }
        }
        catch { }
        return RedirectToAction("Create");
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var taxUpdate = await _dbContext.Taxes.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

            if (taxUpdate != null)
            {
                var taxVMUpdate = GetTaxVMFromtaxModel(taxUpdate);

                return View(taxVMUpdate);
            }
        }
        catch { }
        return null;
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, TaxViewModel taxVM)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var taxUpdate = await _dbContext.Taxes.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

            if (taxUpdate != null)
            {
                taxUpdate.Name = taxVM.Name;
                taxUpdate.Description = taxVM.Description;
                taxUpdate.IsDefault = taxVM.IsDefault;
                taxUpdate.IsActive = taxVM.IsActive;
                taxUpdate.Code = taxVM.Code;
                taxUpdate.Rate = taxVM.Rate;

                _dbContext.Taxes.Update(taxUpdate);

                int result = await _dbContext.SaveChangesAsync();

                if (taxUpdate.IsDefault)
                {
                    await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM TaxProducts WHERE TaxId = {0}", taxUpdate.Id);
                }
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
            var taxDelete = await _dbContext.Taxes.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();
            if (taxDelete != null)
            {
                _dbContext.Taxes.Remove(taxDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
        catch { }
        return RedirectToAction("Index");
    }

    private TaxViewModel GetTaxVMFromtaxModel(TaxModel tax)
    {
        return new TaxViewModel()
        {
            Id = tax.Id,
            Name = tax.Name,
            Code = tax.Code,
            Rate = tax.Rate,
            IsActive = tax.IsActive,
            IsDefault = tax.IsDefault,
            Description = tax.Description,

        };
    }
}