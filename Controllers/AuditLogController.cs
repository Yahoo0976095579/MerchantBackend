// Controllers/AuditLogController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerchantBackend.Data;
using MerchantBackend.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // 引入 Dictionary

namespace MerchantBackend.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        // <--- 新增：ActionType 中文映射字典
        private static readonly Dictionary<string, string> ActionTypeMappings = new Dictionary<string, string>
        {
            {"Login_Success", "登入成功"},
            {"Login_Failure_UnauthorizedRole", "登入失敗 (權限不足)"},
            {"Login_Failure_LockedOut", "登入失敗 (帳號鎖定)"},
            {"Login_Failure_InvalidCredentials", "登入失敗 (憑證無效)"},
            {"Logout_Success", "登出成功"},
            {"User_Created", "使用者創建"},
            {"User_Edited", "使用者編輯"},
            {"User_PasswordChanged", "個人密碼修改"}, // <--- 新增這行

            // <--- 新增分類操作的映射
            {"Category_Created", "分類新增"},
            {"Category_Edited", "分類編輯"},
            {"Category_Deleted", "分類刪除"},
                // <--- 新增標籤操作的映射
            {"Tag_Created", "標籤新增"},
            {"Tag_Edited", "標籤編輯"},
            {"Tag_Deleted", "標籤刪除"},
            // <--- 新增商品操作的映射 (未來會再細分)
            {"Product_Created", "商品新增"},
            {"Product_Edited", "商品編輯"},
            {"Product_Deleted", "商品刪除"},
            {"Product_StatusChanged", "商品狀態變更"}, // 例如上架/下架
            {"Product_ImageUploaded", "商品圖片上傳"},
            {"Product_ImageDeleted", "商品圖片刪除"},
            {"Product_ImageSetMain", "商品主圖設定"},
            // {"User_PasswordChanged", "個人密碼修改"},
            // {"User_EmailChanged", "個人電子郵件修改"}
        };

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string searchString,
            string actionTypeFilter,
            string targetEntityFilter,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber = 1,
            int pageSize = 20)
        {
            ViewData["CurrentSearchString"] = searchString;
            ViewData["CurrentActionTypeFilter"] = actionTypeFilter;
            ViewData["CurrentTargetEntityFilter"] = targetEntityFilter;
            ViewData["CurrentStartDate"] = startDate?.ToString("yyyy-MM-ddTHH:mm");
            ViewData["CurrentEndDate"] = endDate?.ToString("yyyy-MM-ddTHH:mm");
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            var auditLogs = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                auditLogs = auditLogs.Where(log =>
                    log.ActorUsername.Contains(searchString) ||
                    (log.Details != null && log.Details.Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(actionTypeFilter) && actionTypeFilter != "All")
            {
                auditLogs = auditLogs.Where(log => log.ActionType == actionTypeFilter);
            }

            if (!string.IsNullOrEmpty(targetEntityFilter) && targetEntityFilter != "All")
            {
                auditLogs = auditLogs.Where(log => log.TargetEntity == targetEntityFilter);
            }

            if (startDate.HasValue)
            {
                auditLogs = auditLogs.Where(log => log.Timestamp >= startDate.Value.ToUniversalTime());
            }
            if (endDate.HasValue)
            {
                auditLogs = auditLogs.Where(log => log.Timestamp <= endDate.Value.ToUniversalTime());
            }

            // 獲取所有不重複的 ActionType (英文值) 用於下拉選單
            ViewBag.AvailableActionTypes = await _context.AuditLogs
                                                        .Select(l => l.ActionType)
                                                        .Distinct()
                                                        .OrderBy(at => at)
                                                        .ToListAsync();

            ViewBag.AvailableTargetEntities = await _context.AuditLogs
                                                         .Select(l => l.TargetEntity)
                                                         .Distinct()
                                                         .OrderBy(te => te)
                                                         .ToListAsync();

            var totalRecords = await auditLogs.CountAsync();
            var paginatedLogs = await auditLogs
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // <--- 關鍵修改：在 View 中直接使用映射
            // 我們不需要在這裡創建新的 ViewModel 或匿名物件
            // 直接在 View 中處理 ActionType 的顯示邏輯

            return View(paginatedLogs);
        }

        // GET: AuditLog/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var auditLog = await _context.AuditLogs.FindAsync(id);
            if (auditLog == null)
            {
                return NotFound();
            }
            return View(auditLog);
        }

        // <--- 新增一個輔助方法來獲取中文名稱 (View 中也可以直接使用字典)
        public static string GetLocalizedActionType(string actionType)
        {
            if (ActionTypeMappings.TryGetValue(actionType, out string? localizedName))
            {
                return localizedName;
            }
            return actionType; // 如果沒有找到映射，則顯示原始英文名稱
        }
    }
}