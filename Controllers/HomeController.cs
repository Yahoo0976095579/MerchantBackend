// Controllers/HomeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantBackend.Models;
using System.Diagnostics;
using MerchantBackend.Data; // 引入 DbContext
using Microsoft.EntityFrameworkCore; // 引入 CountAsync, AnyAsync 等
using System; // 引入 DateTime
namespace MerchantBackend.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // <--- 注入 DbContext

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context=context;
        }

        public async Task<IActionResult> Index() // 修改為 async Task<IActionResult>
        {
            var dashboard = new DashboardViewModel();

            // 獲取使用者總數
            dashboard.TotalUsers = await _context.Users.CountAsync(); // IdentityUser 總數

            // 獲取商品總數和上架中商品數
            dashboard.TotalProducts = await _context.Products.Where(p => !p.IsDeleted).CountAsync(); // 只計算未邏輯刪除的
            dashboard.ActiveProducts = await _context.Products.Where(p => !p.IsDeleted && p.IsActive).CountAsync();

            // 獲取分類總數
            dashboard.TotalCategories = await _context.Categories.CountAsync();

            // 獲取標籤總數
            dashboard.TotalTags = await _context.Tags.CountAsync();

            // 獲取今日登入次數 (從稽核日誌中計算)
            var today = DateTime.UtcNow.Date;
            dashboard.TodayLogins = await _context.AuditLogs
                                                .Where(log => log.ActionType == "Login_Success" &&
                                                              log.Timestamp.Date == today) // 比較日期部分
                                                .CountAsync();

            return View(dashboard); // 傳遞 DashboardViewModel
        }


        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
