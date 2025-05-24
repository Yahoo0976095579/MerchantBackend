// Controllers/HomeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantBackend.Models;
using System.Diagnostics;
using MerchantBackend.Data; // �ޤJ DbContext
using Microsoft.EntityFrameworkCore; // �ޤJ CountAsync, AnyAsync ��
using System; // �ޤJ DateTime
namespace MerchantBackend.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // <--- �`�J DbContext

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context=context;
        }

        public async Task<IActionResult> Index() // �קאּ async Task<IActionResult>
        {
            var dashboard = new DashboardViewModel();

            // ����ϥΪ��`��
            dashboard.TotalUsers = await _context.Users.CountAsync(); // IdentityUser �`��

            // ����ӫ~�`�ƩM�W�[���ӫ~��
            dashboard.TotalProducts = await _context.Products.Where(p => !p.IsDeleted).CountAsync(); // �u�p�⥼�޿�R����
            dashboard.ActiveProducts = await _context.Products.Where(p => !p.IsDeleted && p.IsActive).CountAsync();

            // ��������`��
            dashboard.TotalCategories = await _context.Categories.CountAsync();

            // ��������`��
            dashboard.TotalTags = await _context.Tags.CountAsync();

            // �������n�J���� (�q�]�֤�x���p��)
            var today = DateTime.UtcNow.Date;
            dashboard.TodayLogins = await _context.AuditLogs
                                                .Where(log => log.ActionType == "Login_Success" &&
                                                              log.Timestamp.Date == today) // ����������
                                                .CountAsync();

            return View(dashboard); // �ǻ� DashboardViewModel
        }


        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
