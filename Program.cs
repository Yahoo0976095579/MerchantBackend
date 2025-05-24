using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MerchantBackend.Data; // �ޤJ�A�� DbContext
using MerchantBackend.SeedData; // �ޤJ�A�� DbInitializer
using MerchantBackend.Services; // �ޤJ�A�� AuditService
using MerchantBackend.IdentityLocalizations; // �ޤJ�A���۩w�q ErrorDescriber

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// �K�[ Identity �A��
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false) // V1 �Ȯɤ��n�D�q�l�l��T�{
    .AddRoles<IdentityRole>() // �K�[����޲z���
    .AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<LocalizedIdentityErrorDescriber>(); // <--- �K�[�o��I
// �t�m Identity ���w�]����
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // �]�w�n�J�������|
    options.AccessDeniedPath = "/Account/AccessDenied"; // �i��G�]�w�s���Q�ڭ������|
    options.LogoutPath = "/Account/Logout"; // �i��G�]�w�n�X�������|
});


// ���U�]�֪A��
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

// �T�O Authentication �b Authorization ���e
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// �����ƺؤl
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
