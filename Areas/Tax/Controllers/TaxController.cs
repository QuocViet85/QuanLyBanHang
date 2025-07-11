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

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            List<TaxModel> taxes;

            if (pageNumber > 0 && limit > 0)
            {
                taxes = await _dbContext.Taxes.Where(t => t.UserId == user.Id)
                                                .Skip((pageNumber - 1) * limit)
                                                .Take(limit)
                                                .ToListAsync();
            }
            else
            {
                taxes = await _dbContext.Taxes.Where(t => t.UserId == user.Id).ToListAsync();
            }

            int totalTaxes = await _dbContext.Taxes.CountAsync();

            List<TaxViewModel> taxVMs = new List<TaxViewModel>();

            if (taxes.Count > 0)
            {
                taxVMs = taxes.Select(p => GetTaxVMFromtaxModel(p)).ToList();
            }
            return Ok(new
            {
                taxes = taxVMs,
                totalTaxes = totalTaxes
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

            var taxes = await _dbContext.Taxes.Where(t => t.UserId == user.Id && t.IsActive).ToListAsync();

            List<TaxViewModel> taxVMs = new List<TaxViewModel>();

            if (taxes.Count > 0)
            {
                taxVMs = taxes.Select(p => GetTaxVMFromtaxModel(p)).ToList();
            }
            return Ok(taxVMs);
        }
        catch
        { return BadRequest("Lấy thuế thất bại"); }
    }

    [HttpGet("active-default")]
    public async Task<IActionResult> GetTaxActiveDefaut()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var taxes = await _dbContext.Taxes.Where(t => t.UserId == user.Id && t.IsActive && t.IsDefault).ToListAsync();

            List<TaxViewModel> taxVMs = new List<TaxViewModel>();

            if (taxes.Count > 0)
            {
                taxVMs = taxes.Select(p => GetTaxVMFromtaxModel(p)).ToList();
            }
            return Ok(taxVMs);
        }
        catch
        { return BadRequest("Lấy thuế thất bại"); }
    }

    [HttpGet("active-private")]
    public async Task<IActionResult> GetTaxActivePrivate()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var taxes = await _dbContext.Taxes.Where(t => t.UserId == user.Id && t.IsActive && !t.IsDefault).ToListAsync();

            List<TaxViewModel> taxVMs = new List<TaxViewModel>();

            if (taxes.Count > 0)
            {
                taxVMs = taxes.Select(p => GetTaxVMFromtaxModel(p)).ToList();
            }
            return Ok(taxVMs);
        }
        catch
        { return BadRequest("Lấy thuế thất bại"); }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] TaxViewModel taxVM)
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
    public async Task<IActionResult> Update(int id, [FromBody] TaxViewModel taxVM)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var taxUpdate = await _dbContext.Taxes.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
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
                    return Ok("Cập nhật thuế thành công");
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
        catch { return BadRequest("Cập nhật thuế thất bại"); }

    }

    [HttpPost("delete/{id}")]
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
                return Ok("Xóa thuế thành công");
            }
            else
            {
                throw new Exception();
            }
        }
        catch { return BadRequest("Xóa thuế thất bại"); }
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