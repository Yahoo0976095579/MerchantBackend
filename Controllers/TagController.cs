// Controllers/TagController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerchantBackend.Data;
using MerchantBackend.Models; // 引入 Tag 模型
using MerchantBackend.Services; // 引入 AuditService
using System.Linq;
using System.Threading.Tasks;
using System; // For TempData

namespace MerchantBackend.Controllers
{
    // Manager 角色可執行所有操作，Editor 角色可查看
    [Authorize(Roles = "Manager, Editor")]
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public TagController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: Tag/Index (所有角色可查看)
        public async Task<IActionResult> Index()
        {
            var tags = await _context.Tags.OrderBy(t => t.Name).ToListAsync();
            return View(tags);
        }

        // GET: Tag/Details/{id} (所有角色可查看)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null) return NotFound();

            return View(tag);
        }

        // GET: Tag/Create (僅 Manager 角色可新增)
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tag/Create (僅 Manager 角色可新增)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("Name")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                // 檢查名稱是否重複
                if (await _context.Tags.AnyAsync(t => t.Name == tag.Name))
                {
                    ModelState.AddModelError("Name", "此標籤名稱已存在。");
                    return View(tag);
                }

                _context.Add(tag);
                await _context.SaveChangesAsync();

                // 記錄稽核日誌
                await _auditService.LogAsync(
                    actionType: "Tag_Created",
                    targetEntity: "Tag",
                    targetEntityId: tag.Id.ToString(),
                    user: User,
                    details: new { tag.Name },
                    outcome: "Success"
                );

                TempData["SuccessMessage"] = $"標籤 '{tag.Name}' 創建成功！";
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tag/Edit/{id} (僅 Manager 角色可編輯)
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            return View(tag);
        }

        // POST: Tag/Edit/{id} (僅 Manager 角色可編輯)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Tag tag)
        {
            if (id != tag.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 獲取原始數據以便日誌記錄
                    var originalTag = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                    if (originalTag == null) return NotFound();

                    // 檢查名稱是否重複 (除了自己)
                    if (await _context.Tags.AnyAsync(t => t.Name == tag.Name && t.Id != tag.Id))
                    {
                        ModelState.AddModelError("Name", "此標籤名稱已存在。");
                        return View(tag);
                    }

                    _context.Update(tag);
                    await _context.SaveChangesAsync();

                    // 記錄稽核日誌
                    await _auditService.LogAsync(
                        actionType: "Tag_Edited",
                        targetEntity: "Tag",
                        targetEntityId: tag.Id.ToString(),
                        user: User,
                        details: new { Original = new { originalTag.Name }, Updated = new { tag.Name } },
                        outcome: "Success"
                    );

                    TempData["SuccessMessage"] = $"標籤 '{tag.Name}' 更新成功！";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tag/Delete/{id} (僅 Manager 角色可刪除)
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null) return NotFound();

            return View(tag);
        }

        // POST: Tag/Delete/{id} (僅 Manager 角色可刪除)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                // 記錄稽核日誌 (在刪除前獲取數據)
                await _auditService.LogAsync(
                    actionType: "Tag_Deleted",
                    targetEntity: "Tag",
                    targetEntityId: tag.Id.ToString(),
                    user: User,
                    details: new { tag.Name },
                    outcome: "Success"
                );

                _context.Tags.Remove(tag);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"標籤 '{tag?.Name}' 已刪除！";
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}