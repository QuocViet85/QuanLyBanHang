using Microsoft.EntityFrameworkCore;
using Serilog;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()  // Log mức debug trở lên
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Log vào file mỗi ngày 1 file
        .WriteTo.Console() // Log ra console
        .CreateLogger();

        // Gắn Serilog vào ASP.NET Core pipeline
        builder.Host.UseSerilog();

        builder.Services.SetService(connectionString);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "MyArea",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        app.MapFallbackToController("Index", "Home");

        app.MapRazorPages();

        app.Run();
    }
}