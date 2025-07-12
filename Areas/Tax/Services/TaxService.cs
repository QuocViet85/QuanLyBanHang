using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Tax.Model;
using WebBanHang.Areas.Tax.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Tax.Services;

public class TaxService : ITaxService
{
    private readonly ApplicationDbContext _dbContext;
    public TaxService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<(List<TaxVM> taxVMs, int totalTaxes)> GetTaxes(int pageNumber, int limit, string userId)
    {
        List<TaxModel> taxes;

        if (pageNumber > 0 && limit > 0)
        {
            taxes = await _dbContext.Taxes.Where(t => t.UserId == userId)
                                            .Skip((pageNumber - 1) * limit)
                                            .Take(limit)
                                            .ToListAsync();
        }
        else
        {
            taxes = await _dbContext.Taxes.Where(t => t.UserId == userId).ToListAsync();
        }

        int totalTaxes = await _dbContext.Taxes.CountAsync();

        List<TaxVM> taxVMs = new List<TaxVM>();

        if (taxes.Count > 0)
        {
            taxVMs = taxes.Select(p => GetTaxVMFromtaxModel(p)).ToList();
        }
        return (taxVMs, totalTaxes);
    }

    public async Task<List<TaxVM>> GetTaxActive(string userId)
    {
        var taxes = await _dbContext.Taxes.Where(t => t.UserId == userId && t.IsActive).ToListAsync();

        List<TaxVM> taxVMs = new List<TaxVM>();

        if (taxes.Count > 0)
        {
            taxVMs = taxes.Select(p => GetTaxVMFromtaxModel(p)).ToList();
        }

        return taxVMs;
    }

    public async Task Create(TaxVM taxVM, string userId)
    {
        var taxModel = new TaxModel()
        {
            Name = taxVM.Name,
            Description = taxVM.Description,
            Code = taxVM.Code,
            IsActive = taxVM.IsActive,
            IsDefault = taxVM.IsDefault,
            UserId = userId,
            Rate = taxVM.Rate,
        };

        await _dbContext.Taxes.AddAsync(taxModel);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(int id, TaxVM taxVM, string userId)
    {
        var taxUpdate = await _dbContext.Taxes.Where(p => p.Id == id && p.UserId == userId).FirstOrDefaultAsync();

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
        else
        {
            throw new Exception();
        }
    }

    public async Task Delete(int id, string userId)
    {
        var taxDelete = await _dbContext.Taxes.Where(p => p.Id == id && p.UserId == userId).FirstOrDefaultAsync();
        if (taxDelete != null)
        {
            _dbContext.Taxes.Remove(taxDelete);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception();
        }
    }

    public TaxVM GetTaxVMFromtaxModel(TaxModel tax)
    {
        return new TaxVM()
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