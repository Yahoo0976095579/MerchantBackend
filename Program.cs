using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MerchantBackend.Data; // 引入你的 DbContext
using MerchantBackend.SeedData; // 引入你的 DbInitializer
using MerchantBackend.Services; // 引入你的 AuditService
using MerchantBackend.IdentityLocalizations; // 引入你的自定義 ErrorDescriber

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// 添加 Identity 服務
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false) // V1 暫時不要求電子郵件確認
    .AddRoles<IdentityRole>() // 添加角色管理支持
    .AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<LocalizedIdentityErrorDescriber>(); // <--- 添加這行！
// 配置 Identity 的預設路由
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // 設定登入頁面路徑
    options.AccessDeniedPath = "/Account/AccessDenied"; // 可選：設定存取被拒頁面路徑
    options.LogoutPath = "/Account/Logout"; // 可選：設定登出頁面路徑
});


// 註冊稽核服務
builder.Services.AddScoped<IAuditService, AuditService>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 確保 Authentication 在 Authorization 之前
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 執行資料種子
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        await DbInitializer.Initialize(serviceProvider);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();
