using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.Product.Model;
using WebBanHang.Areas.Customer.Model;
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

            entity.HasOne(cp => cp.Product) //chỉ ra đối tượng có mqh 1 nhiều với bảng này trên bảng này
                .WithMany(p => p.CategoryProducts) //chỉ ra đối tượng là bảng này trên bảng có mối quan hệ 1 nhiều với bảng này
                .HasForeignKey(cp => cp.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Category_Product");
        });

        builder.Entity<AttributeProductModel>(entity =>
        {
            entity.HasKey(ap => new { ap.ProductId, ap.AttributeId });

            entity.HasOne(ap => ap.Product) //chỉ ra đối tượng có mqh 1 nhiều với bảng này trên bảng này
                .WithMany(p => p.AttributeProducts) //chỉ ra đối tượng là bảng này trên bảng có mối quan hệ 1 nhiều với bảng này
                .HasForeignKey(ap => ap.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Attribute_Product");
        });

        builder.Entity<OrderProductModel>(entity =>
        {
            entity.HasKey(op => new { op.ProductId, op.OrderId });

            entity.HasOne(op => op.Product) //chỉ ra đối tượng có mqh 1 nhiều với bảng này trên bảng này
                .WithMany(p => p.OrderProducts) //chỉ ra đối tượng là bảng này trên bảng có mối quan hệ 1 nhiều với bảng này
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Order_Product");
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
