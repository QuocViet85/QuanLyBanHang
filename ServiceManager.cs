using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Services;
using WebBanHang.Areas.DynamicAttribute.Services;
using WebBanHang.Areas.Order.Services;
using WebBanHang.Areas.Product.Services;
using WebBanHang.Areas.Tax.Services;
using WebBanHang.Data;

public static class ServiceManager
{
    public static void SetService(this IServiceCollection services, string connectionString)
    {
        SetAuth(services, connectionString);
        SetResourceService(services);
    }
    public static void SetAuth(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false; // Không bắt phải có số
            options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
            options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
            options.Password.RequireUppercase = false; // Không bắt buộc chữ in
            options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
            options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

            // Cấu hình đăng nhập.
            options.SignIn.RequireConfirmedEmail = false;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
            options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
            options.SignIn.RequireConfirmedAccount = false;         // Xác thực tài khoản
        });
    }

    public static void SetResourceService(IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ITaxService, TaxService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IDynamicAttributeService, DynamicAttributeService>();
        services.AddScoped<ICategoryService, CategoryService>();
    }
}