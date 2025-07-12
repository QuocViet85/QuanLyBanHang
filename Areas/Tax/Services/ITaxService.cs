using WebBanHang.Areas.Tax.Model;
using WebBanHang.Areas.Tax.ViewModel;

namespace WebBanHang.Areas.Tax.Services;

public interface ITaxService
{
    public Task<(List<TaxVM> taxVMs, int totalTaxes)> GetTaxes(int pageNumber, int limit, string userId);

    public Task<List<TaxVM>> GetTaxActive(string userId);

    public Task Create(TaxVM TaxVM, string userId);

    public Task Update(int id, TaxVM TaxVM, string userId);

    public Task Delete(int id, string userId);

    public TaxVM GetTaxVMFromtaxModel(TaxModel tax);
}