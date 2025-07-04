namespace WebBanHang.Areas.Tax.ViewModel;

public class TaxViewModel
{
    public int Id { set; get; }
    public string Name { set; get; }
    public string? Code { set; get; }
    public decimal Rate { set; get; }
    public string? Description { set; get; }
    public bool IsActive { set; get; } = true;
    public bool IsDefault { set; get; } = true;
}