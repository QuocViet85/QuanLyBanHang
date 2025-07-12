using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.DynamicAttribute.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.DynamicAttribute.Services;

public class DynamicAttributeService : IDynamicAttributeService
{
    private readonly ApplicationDbContext _dbContext;

    public DynamicAttributeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<DynamicAttributeVM> dynamicAttributeVMs, int totalDynamicAttributes)> GetDynamicAttributes(int pageNumber, int limit, string userId)
    {
        List<DynamicAttributeModel> dynamicAttributes;

        if (pageNumber > 0 && limit > 0)
        {
            dynamicAttributes = await _dbContext.Attributes.Where(a => a.UserId == userId)
                                                    .Skip((pageNumber - 1) * limit)
                                                    .Take(limit)
                                                    .ToListAsync();
        }
        else
        {
            dynamicAttributes = await _dbContext.Attributes.Where(a => a.UserId == userId).ToListAsync();
        }

        int totalDynamicAttributes = await _dbContext.Attributes.CountAsync();

        List<DynamicAttributeVM> dynamicAttributeVMs = new List<DynamicAttributeVM>();

        if (dynamicAttributes?.Count > 0)
        {
            dynamicAttributeVMs = dynamicAttributes.Select(a => GetDynamicAttributeVMFromAttribute(a)).ToList();
        }

        return (dynamicAttributeVMs, totalDynamicAttributes);
    }
    public async Task Create(DynamicAttributeVM dynamicAttributeVM, string userId)
    {
        var dynamicAttributeModel = new DynamicAttributeModel()
        {
            Name = dynamicAttributeVM.Name,
            UserId = userId
        };

        await _dbContext.Attributes.AddAsync(dynamicAttributeModel);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(int id, DynamicAttributeVM dynamicAttributeVM, string userId)
    {
        var dynamicAttributeUpdate = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == userId).FirstOrDefaultAsync();

        if (dynamicAttributeUpdate != null)
        {
            dynamicAttributeUpdate.Name = dynamicAttributeVM.Name;

            _dbContext.Attributes.Update(dynamicAttributeUpdate);
            int result = await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception();
        }
    }

    public async Task Delete(int id, string userId)
    {
        var attributeDelete = await _dbContext.Attributes.Where(a => a.Id == id && a.UserId == userId).FirstOrDefaultAsync();
        if (attributeDelete != null)
        {
            _dbContext.Attributes.Remove(attributeDelete);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception();
        }
    }

    public DynamicAttributeVM GetDynamicAttributeVMFromAttribute(DynamicAttributeModel attribute)
    {
        return new DynamicAttributeVM()
        {
            Id = attribute.Id,
            Name = attribute.Name
        };
    }
}