using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.Product.Model;
using WebBanHang.Customer.Model;
using WebBanHang.Areas.Order.Model;

namespace WebBanHang.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CategoryProductModel>(entity =>
        {
            entity.HasKey(ct => new { ct.ProductId, ct.CategoryId });
        });

        builder.Entity<AttributeProductModel>(entity =>
        {
            entity.HasKey(ap => new { ap.ProductId, ap.AttributeId });
        });
        
        builder.Entity<OrderProductModel>(entity =>
        {
            entity.HasKey(op => new { op.ProductId, op.OrderId});
        });
    }

    public DbSet<ProductModel> Products { set; get; }
    public DbSet<ProductPhotoModel> ProductPhotos { set; get; }
    public DbSet<CategoryModel> Categories { set; get; }
    public DbSet<CategoryProductModel> CategoryProducts { set; get; }
    public DbSet<AttributeModel> Attributes { set; get; }
    public DbSet<AttributeProductModel> AttributeProducts { set; get; }
    public DbSet<AttributeValueModel> AttributeValues { set; get; }
    public DbSet<OrderModel> Orders { set; get; }
    public DbSet<OrderProductModel> OrderProducts { set; get; }
    public DbSet<CustomerModel> Customers { set; get; }
}
