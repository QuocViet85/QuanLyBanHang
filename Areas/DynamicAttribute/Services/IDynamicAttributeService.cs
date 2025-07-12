using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.DynamicAttribute.ViewModel;

namespace WebBanHang.Areas.DynamicAttribute.Services;

public interface IDynamicAttributeService
{
    public Task<(List<DynamicAttributeVM> dynamicAttributeVMs, int totalDynamicAttributes)> GetDynamicAttributes(int pageNumber, int limit, string userId);

    public Task Create(DynamicAttributeVM dynamicAttributeVM, string userId);

    public Task Update(int id, DynamicAttributeVM dynamicAttributeVM, string userId);

    public Task Delete(int id, string userId);

    public DynamicAttributeVM GetDynamicAttributeVMFromAttribute(DynamicAttributeModel attribute);
}