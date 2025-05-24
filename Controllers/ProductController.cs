// Controllers/ProductController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerchantBackend.Data;
using MerchantBackend.Models.Products; // 引入 Product, Category, Tag 相關模型
using MerchantBackend.Models; // 引入 ProductListViewModel
using MerchantBackend.Services; // 引入 AuditService
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; // 用於 SelectListItem
using Microsoft.Extensions.Configuration; // 用於讀取配置

namespace MerchantBackend.Controllers
{
    // Manager 或 Editor 角色可存取商品管理相關功能
    [Authorize(Roles = "Manager, Editor")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IConfiguration _configuration; // 注入配置服務

        public ProductController(
            ApplicationDbContext context,
            IAuditService auditService,
            IConfiguration configuration)
        {
            _context = context;
            _auditService = auditService;
            _configuration = configuration;
        }

        // GET: Product/Index
        // GET: Product/Index
        public async Task<IActionResult> Index(
            string searchString,
            int? categoryFilterId,
            int? tagFilterId,
            bool? isActiveFilter, // 傳入的布林值
            int pageNumber = 1,
            int pageSize = 10)
        {
            ViewData["CurrentSearchString"] = searchString;
            ViewData["CurrentCategoryFilterId"] = categoryFilterId;
            ViewData["CurrentTagFilterId"] = tagFilterId;
            ViewData["CurrentIsActiveFilter"] = isActiveFilter; // 仍然需要這個來預選
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            // 獲取所有分類和標籤用於篩選下拉選單
            ViewBag.AvailableCategories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", categoryFilterId);
            ViewBag.AvailableTags = new SelectList(await _context.Tags.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", tagFilterId);

            // <--- 新增這裡：為上架狀態創建 SelectList
            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "所有", Selected = !isActiveFilter.HasValue }, // 如果沒有篩選，預設選中「所有」
                new SelectListItem { Value = "true", Text = "上架中", Selected = isActiveFilter.HasValue && isActiveFilter.Value == true },
                new SelectListItem { Value = "false", Text = "已下架", Selected = isActiveFilter.HasValue && isActiveFilter.Value == false }
            };
            ViewBag.AvailableStatusOptions = statusOptions; // 將這個列表傳遞給 View

            // 查詢商品，包含相關聯的 Category 和 Images
            var productsQuery = _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Images.Where(img => img.IsMain)) // 只包含主圖片
                                .Include(p => p.ProductTags!) // 包含 ProductTags
                                    .ThenInclude(pt => pt.Tag) // 進一步包含 Tag
                                .Where(p => !p.IsDeleted) // 只顯示未邏輯刪除的商品
                                .AsQueryable();

            // 搜尋 (商品名稱或描述)
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(searchString) ||
                    (p.Description != null && p.Description.Contains(searchString)));
            }

            // 篩選 (依分類)
            if (categoryFilterId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryFilterId.Value);
            }

            // 篩選 (依標籤)
            if (tagFilterId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.ProductTags!.Any(pt => pt.TagId == tagFilterId.Value));
            }

            // 篩選 (依上架狀態)
            if (isActiveFilter.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.IsActive == isActiveFilter.Value);
            }

            var totalProducts = await productsQuery.CountAsync();
            var paginatedProducts = await productsQuery
                .OrderByDescending(p => p.CreatedAt) // 最新商品在前
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productViewModels = new List<ProductListViewModel>();
            foreach (var product in paginatedProducts)
            {
                var mainImage = product.Images?.FirstOrDefault(); // 應該只有一個 IsMain 為 true 的圖片
                var tagsList = product.ProductTags?.Select(pt => pt.Tag?.Name).Where(name => name != null).ToList();

                productViewModels.Add(new ProductListViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description, // <--- 新增：填充 Description
                    Price = product.Price,
                    Stock = product.Stock,
                    IsActive = product.IsActive,
                    CategoryName = product.Category?.Name, // 顯示分類名稱
                    Tags = tagsList != null && tagsList.Any() ? string.Join(", ", tagsList) : null, // 顯示標籤名稱
                    MainImagePath = mainImage != null ? GetProductImageUrl(mainImage.ImagePath) : "/images/placeholder.png" // 如果沒有主圖，顯示預設圖片
                });
            }

            ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewBag.PageSize = pageSize;

            return View(productViewModels);
        }

        // GET: Product/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // 獲取所有分類和標籤用於下拉選單
            ViewBag.AvailableCategories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
            ViewBag.AvailableTags = new SelectList(await _context.Tags.OrderBy(t => t.Name).ToListAsync(), "Id", "Name");

            return View(new ProductCreateViewModel());
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            // 在驗證模型時，我們不需要驗證圖片檔案本身是否為必填，因為可能沒有圖片
            // 但其他商品屬性是必填的。
            // 為了避免因 IFormFile 導致 ModelState 驗證失敗，我們可以稍微調整

            // 如果沒有選擇任何檔案，清除 ModelState 中 ImageFiles 的錯誤
            if (model.ImageFiles == null || !model.ImageFiles.Any())
            {
                ModelState.Remove(nameof(model.ImageFiles));
            }

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Stock = model.Stock,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CategoryId = model.SelectedCategoryId
                };

                _context.Add(product);
                await _context.SaveChangesAsync(); // 先保存 Product 以獲得 Id

                // 處理圖片上傳
                if (model.ImageFiles != null && model.ImageFiles.Any())
                {
                    var uploadedImages = new List<ProductImage>();
                    string uploadFolder = _configuration.GetValue<string>("FileUploadSettings:ProductImageUploadFolder") ?? "images/products";
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolder);

                    // 如果資料夾不存在，則創建
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    int sortOrder = 0;
                    // 預設將第一張圖片設置為主圖
                    bool hasMainImage = false;

                    foreach (var file in model.ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            // 確保檔案名唯一，避免覆蓋
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImagePath = uniqueFileName, // 儲存相對路徑 (文件名)
                                IsMain = !hasMainImage, // 第一張設為主圖
                                SortOrder = sortOrder++
                            };
                            uploadedImages.Add(productImage);
                            if (!hasMainImage) hasMainImage = true; // 設置第一張為主圖後，後續的就不是
                        }
                    }

                    if (uploadedImages.Any())
                    {
                        await _context.ProductImages.AddRangeAsync(uploadedImages);
                        await _context.SaveChangesAsync(); // 保存圖片
                    }
                }

                // 處理標籤關聯
                if (model.SelectedTagIds != null && model.SelectedTagIds.Any())
                {
                    var productTags = new List<ProductTag>();
                    foreach (var tagId in model.SelectedTagIds)
                    {
                        productTags.Add(new ProductTag { ProductId = product.Id, TagId = tagId });
                    }
                    await _context.ProductTags.AddRangeAsync(productTags);
                    await _context.SaveChangesAsync(); // 保存標籤關聯
                }

                // 記錄稽核日誌
                await _auditService.LogAsync(
                    actionType: "Product_Created",
                    targetEntity: "Product",
                    targetEntityId: product.Id.ToString(),
                    user: User,
                    details: new
                    {
                        product.Name,
                        product.Price,
                        product.Stock,
                        product.IsActive,
                        CategoryId = product.CategoryId,
                        Tags = model.SelectedTagIds,
                        ImageCount = model.ImageFiles?.Count ?? 0
                    },
                    outcome: "Success"
                );

                TempData["SuccessMessage"] = $"商品 '{product.Name}' 創建成功！";
                return RedirectToAction(nameof(Index));
            }

            // 如果模型驗證失敗，重新加載分類和標籤以便返回 View
            ViewBag.AvailableCategories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", model.SelectedCategoryId);
            ViewBag.AvailableTags = new SelectList(await _context.Tags.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", model.SelectedTagIds);

            return View(model);
        }

        // GET: Product/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Images.OrderBy(img => img.SortOrder)) // 獲取所有圖片並排序
                                .Include(p => p.ProductTags!)
                                    .ThenInclude(pt => pt.Tag)
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || product.IsDeleted) return NotFound(); // 如果商品不存在或已邏輯刪除，則找不到

            // 獲取所有分類和標籤用於下拉選單
            ViewBag.AvailableCategories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", product.CategoryId);
            ViewBag.AvailableTags = new SelectList(await _context.Tags.OrderBy(t => t.Name).ToListAsync(), "Id", "Name"); // Tag 不預選，因為多選框預選複雜

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                SelectedCategoryId = product.CategoryId,
                SelectedTagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new List<int>(),
                ExistingImages = product.Images?.Select(img => new ProductImageViewModel
                {
                    Id = img.Id,
                    ImagePath = GetProductImageUrl(img.ImagePath), // 轉換為完整 URL 顯示
                    SortOrder = img.SortOrder
                }).ToList() ?? new List<ProductImageViewModel>(),
                MainImageId = product.Images?.FirstOrDefault(img => img.IsMain)?.Id // <-- 新增：設定主圖的 Id
            };

            return View(model);
        }

        // POST: Product/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            // 在驗證模型時，我們不需要驗證圖片檔案本身是否為必填
            if (model.NewImageFiles == null || !model.NewImageFiles.Any())
            {
                ModelState.Remove(nameof(model.NewImageFiles));
            }
            if (model.ExistingImages == null) // 防止空引用
            {
                model.ExistingImages = new List<ProductImageViewModel>();
            }

            // <--- 關鍵修改：在 ModelState.IsValid 外部定義 uploadFolder
            // 這樣在 ModelState 失敗時也能使用
            string uploadFolder = _configuration.GetValue<string>("FileUploadSettings:ProductImageUploadFolder") ?? "images/products";
            // string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolder); // 這行暫時不需要在這裡，因為只有在寫入檔案時才需要


            if (ModelState.IsValid)
            {
                var product = await _context.Products
                                    .Include(p => p.Images) // 包含所有圖片以便管理
                                    .Include(p => p.ProductTags) // 包含所有標籤以便管理
                                    .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (product == null || product.IsDeleted) return NotFound();

                // 儲存原始數據以便日誌記錄
                var originalProduct = new
                {
                    product.Name,
                    product.Description,
                    product.Price,
                    product.Stock,
                    product.IsActive,
                    product.CategoryId,
                    OriginalTags = product.ProductTags?.Select(pt => pt.TagId).ToList(),
                    OriginalImages = product.Images?.Select(img => new { img.ImagePath, img.IsMain, img.Id }).ToList() // 記錄原始 Id
                };

                // 更新基本屬性
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Stock = model.Stock;
                product.IsActive = model.IsActive;
                product.CategoryId = model.SelectedCategoryId;
                product.UpdatedAt = DateTime.UtcNow;


                // 處理現有圖片的刪除和主圖設定
                //var imagesToKeep = new List<ProductImage>();

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolder); // <--- 確保這裡使用上面定義的 uploadFolder
                // 找出要刪除的舊圖片
                var deletedImageIds = model.ExistingImages.Where(img => img.IsMarkedForDeletion).Select(img => img.Id).ToList();
                foreach (var imgId in deletedImageIds)
                {
                    var imageToDelete = product.Images?.FirstOrDefault(img => img.Id == imgId);
                    if (imageToDelete != null)
                    {
                        var imagePath = Path.Combine(uploadsFolder, imageToDelete.ImagePath);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath); // 刪除物理檔案
                        }
                        _context.ProductImages.Remove(imageToDelete);
                        // 記錄稽核日誌：圖片刪除
                        await _auditService.LogAsync(
                            actionType: "Product_ImageDeleted",
                            targetEntity: "Product",
                            targetEntityId: product.Id.ToString(),
                            user: User,
                            details: new { ProductId = product.Id, ImagePath = imageToDelete.ImagePath },
                            outcome: "Success"
                        );
                    }
                }

                // <--- 關鍵修改：根據 model.MainImageId 設定主圖
                if (product.Images != null)
                {
                    foreach (var img in product.Images)
                    {
                        bool wasMain = img.IsMain; // 記錄原始主圖狀態
                        img.IsMain = (img.Id == model.MainImageId); // 設定主圖

                        // 記錄主圖設定變更
                        if (img.IsMain && !wasMain)
                        {
                            await _auditService.LogAsync(
                                actionType: "Product_ImageSetMain",
                                targetEntity: "Product",
                                targetEntityId: product.Id.ToString(),
                                user: User,
                                details: new { ProductId = product.Id, NewMainImagePath = img.ImagePath },
                                outcome: "Success"
                            );
                        }
                    }
                    // 如果刪除了所有圖片，或者沒有設定主圖，則需要檢查並設定
                    if (!product.Images.Any(img => img.IsMain) && product.Images.Any())
                    {
                        var firstImage = product.Images.OrderBy(img => img.SortOrder).ThenBy(img => img.Id).FirstOrDefault();
                        if (firstImage != null)
                        {
                            firstImage.IsMain = true;
                            await _auditService.LogAsync(
                                actionType: "Product_ImageSetMain",
                                targetEntity: "Product",
                                targetEntityId: product.Id.ToString(),
                                user: User,
                                details: new { ProductId = product.Id, NewMainImagePath = firstImage.ImagePath, Reason = "Auto-set as first available image" },
                                outcome: "Success"
                            );
                        }
                    }
                }


                // 處理新圖片上傳 (保持不變)
                if (model.NewImageFiles != null && model.NewImageFiles.Any())
                {
                    int newImageSortOrder = product.Images?.Any() == true ? product.Images.Max(img => img.SortOrder) + 1 : 0;
                    foreach (var file in model.NewImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImagePath = uniqueFileName,
                                IsMain = !product.Images!.Any(img => img.IsMain), // 如果之前沒有主圖，且新上傳的第一張是，則設為主圖
                                SortOrder = newImageSortOrder++
                            };
                            product.Images!.Add(productImage); // 注意 nullability
                            await _auditService.LogAsync(
                                actionType: "Product_ImageUploaded",
                                targetEntity: "Product",
                                targetEntityId: product.Id.ToString(),
                                user: User,
                                details: new { ProductId = product.Id, ImagePath = productImage.ImagePath, IsMain = productImage.IsMain },
                                outcome: "Success"
                            );
                        }
                    }
                }

                // 處理標籤關聯 (移除舊的，添加新的) (保持不變)
                _context.ProductTags.RemoveRange(product.ProductTags!);
                if (model.SelectedTagIds != null && model.SelectedTagIds.Any())
                {
                    var newProductTags = new List<ProductTag>();
                    foreach (var tagId in model.SelectedTagIds)
                    {
                        newProductTags.Add(new ProductTag { ProductId = product.Id, TagId = tagId });
                    }
                    await _context.ProductTags.AddRangeAsync(newProductTags);
                }

                _context.Update(product);
                await _context.SaveChangesAsync();

                // 記錄稽核日誌
                var updatedProduct = new
                {
                    product.Name,
                    product.Description,
                    product.Price,
                    product.Stock,
                    product.IsActive,
                    product.CategoryId,
                    UpdatedTags = model.SelectedTagIds,
                    UpdatedImages = product.Images?.Select(img => new { img.ImagePath, img.IsMain, img.Id }).ToList()
                };

                await _auditService.LogAsync(
                    actionType: "Product_Edited",
                    targetEntity: "Product",
                    targetEntityId: product.Id.ToString(),
                    user: User,
                    details: new { Original = originalProduct, Updated = updatedProduct },
                    outcome: "Success"
                );

                TempData["SuccessMessage"] = $"商品 '{product.Name}' 更新成功！";
                return RedirectToAction(nameof(Index));
            }

            // 如果模型驗證失敗，重新加載分類和標籤以便返回 View
            ViewBag.AvailableCategories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", model.SelectedCategoryId);
            ViewBag.AvailableTags = new SelectList(await _context.Tags.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", model.SelectedTagIds); // 多選框預選邏輯

            // <--- 關鍵修正：重新構建 ExistingImages 的 ImagePath
            model.ExistingImages = model.ExistingImages?.Select(img => new ProductImageViewModel
            {
                Id = img.Id,
                ImagePath = GetProductImageUrl(img.ImagePath.Replace($"/{uploadFolder}/", "")), // 移除 base path 獲取原始文件名
                SortOrder = img.SortOrder
                // IsMain 屬性已從 ProductImageViewModel 移除
            }).ToList() ?? new List<ProductImageViewModel>();

            return View(model);
        }

        // 私有輔助方法：構建完整的圖片 URL (保持不變)
        private string GetProductImageUrl(string imagePath)
        {
            var uploadFolder = _configuration.GetValue<string>("FileUploadSettings:ProductImageUploadFolder");
            return $"/{uploadFolder}/{imagePath}";
        }

        // 私有輔助方法：刪除物理圖片檔案
        private void DeletePhysicalImage(string fileName)
        {
            string uploadFolder = _configuration.GetValue<string>("FileUploadSettings:ProductImageUploadFolder") ?? "images/products";
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolder);
            string filePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // GET: Product/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Images.Where(img => img.IsMain)) // 只包含主圖片
                                .Include(p => p.ProductTags!)
                                    .ThenInclude(pt => pt.Tag)
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || product.IsDeleted) return NotFound(); // 如果商品不存在或已經邏輯刪除，則找不到

            // 為了在 View 中顯示商品信息，使用一個 ProductListViewModel
            var mainImage = product.Images?.FirstOrDefault();
            var tagsList = product.ProductTags?.Select(pt => pt.Tag?.Name).Where(name => name != null).ToList();

            var model = new ProductListViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CategoryName = product.Category?.Name,
                Tags = tagsList != null && tagsList.Any() ? string.Join(", ", tagsList) : null,
                MainImagePath = mainImage != null ? GetProductImageUrl(mainImage.ImagePath) : "/images/placeholder.png"
            };

            return View(model);
        }

        // POST: Product/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null || product.IsDeleted)
            {
                // 如果商品不存在或已經邏輯刪除，重定向回列表
                TempData["ErrorMessage"] = "商品不存在或已邏輯刪除。";
                return RedirectToAction(nameof(Index));
            }

            // 執行邏輯刪除
            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow; // 更新時間戳

            _context.Update(product); // 更新實體
            await _context.SaveChangesAsync();

            // 記錄稽核日誌
            await _auditService.LogAsync(
                actionType: "Product_Deleted", // 操作類型為邏輯刪除
                targetEntity: "Product",
                targetEntityId: product.Id.ToString(),
                user: User,
                details: new { product.Name, product.Description, Action = "Logical Delete (IsDeleted = true)" }, // 記錄邏輯刪除
                outcome: "Success"
            );

            TempData["SuccessMessage"] = $"商品 '{product.Name}' 已成功邏輯刪除！";
            return RedirectToAction(nameof(Index));
        }
        // GET: Product/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                                .Include(p => p.Category) // 包含分類
                                .Include(p => p.Images.OrderBy(img => img.SortOrder)) // 包含所有圖片並排序
                                .Include(p => p.ProductTags!) // 包含 ProductTags
                                    .ThenInclude(pt => pt.Tag) // 進一步包含 Tag
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || product.IsDeleted) return NotFound(); // 如果商品不存在或已邏輯刪除，則找不到

            // 將 Product 實體映射到一個適合顯示的 ViewModel (或者直接使用 Product 實體)
            // 這裡我們直接使用 Product 實體，因為它包含所有所需數據
            // 但我們需要將圖片路徑轉換為完整 URL

            // 處理圖片路徑
            if (product.Images != null)
            {
                foreach (var image in product.Images)
                {
                    image.ImagePath = GetProductImageUrl(image.ImagePath); // 轉換為完整 URL
                }
            }

            return View(product); // 直接傳遞 Product 實體
        }

    }
}