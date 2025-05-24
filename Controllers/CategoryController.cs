// Controllers/CategoryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerchantBackend.Data;
using MerchantBackend.Models;
using MerchantBackend.Services; // 引入 AuditService
using System.Linq;
using System.Threading.Tasks;
using System; // For TempData

namespace MerchantBackend.Controllers
{
    // Manager 角色可執行所有操作，Editor 角色可查看
    [Authorize(Roles = "Manager, Editor")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public CategoryController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: Category/Index (所有角色可查看)
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }

        // GET: Category/Details/{id} (所有角色可查看)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // GET: Category/Create (僅 Manager 角色可新增)
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create (僅 Manager 角色可新增)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                // 檢查名稱是否重複
                if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
                {
                    ModelState.AddModelError("Name", "此分類名稱已存在。");
                    return View(category);
                }

                _context.Add(category);
                await _context.SaveChangesAsync();

                // 記錄稽核日誌
                await _auditService.LogAsync(
                    actionType: "Category_Created",
                    targetEntity: "Category",
                    targetEntityId: category.Id.ToString(),
                    user: User,
                    details: new { category.Name, category.Description },
                    outcome: "Success"
                );

                TempData["SuccessMessage"] = $"分類 '{category.Name}' 創建成功！";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/{id} (僅 Manager 角色可編輯)
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Category/Edit/{id} (僅 Manager 角色可編輯)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 獲取原始數據以便日誌記錄
                    var originalCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (originalCategory == null) return NotFound();

                    // 檢查名稱是否重複 (除了自己)
                    if (await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != category.Id))
                    {
                        ModelState.AddModelError("Name", "此分類名稱已存在。");
                        return View(category);
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    // 記錄稽核日誌
                    await _auditService.LogAsync(
                        actionType: "Category_Edited",
                        targetEntity: "Category",
                        targetEntityId: category.Id.ToString(),
                        user: User,
                        details: new { Original = new { originalCategory.Name, originalCategory.Description }, Updated = new { category.Name, category.Description } },
                        outcome: "Success"
                    );

                    TempData["SuccessMessage"] = $"分類 '{category.Name}' 更新成功！";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Category/Delete/{id} (僅 Manager 角色可刪除)
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Category/Delete/{id} (僅 Manager 角色可刪除)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                // 記錄稽核日誌 (在刪除前獲取數據)
                await _auditService.LogAsync(
                    actionType: "Category_Deleted",
                    targetEntity: "Category",
                    targetEntityId: category.Id.ToString(),
                    user: User,
                    details: new { category.Name, category.Description },
                    outcome: "Success"
                );

                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"分類 '{category?.Name}' 已刪除！";
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}