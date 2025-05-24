// Models/ProductEditViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MerchantBackend.Models.Products; // 引入 ProductImage

namespace MerchantBackend.Models
{
    public class ProductEditViewModel
    {
        public int Id { get; set; } // 商品 ID

        [Required(ErrorMessage = "商品名稱為必填項。")]
        [MaxLength(256)]
        [Display(Name = "商品名稱")]
        public string Name { get; set; }

        [MaxLength(1000)]
        [Display(Name = "商品描述")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "價格為必填項。")]
        [Range(0.01, double.MaxValue, ErrorMessage = "價格必須大於零。")]
        [Display(Name = "價格")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "庫存為必填項。")]
        [Range(0, int.MaxValue, ErrorMessage = "庫存不能為負數。")]
        [Display(Name = "庫存")]
        public int Stock { get; set; }

        [Display(Name = "是否上架")]
        public bool IsActive { get; set; }

        // 分類選擇
        [Display(Name = "分類")]
        public int? SelectedCategoryId { get; set; }
        public IEnumerable<SelectListItem>? AvailableCategories { get; set; }

        // 標籤選擇 (多選)
        [Display(Name = "標籤")]
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? AvailableTags { get; set; }

        // 現有圖片列表
        [Display(Name = "現有圖片")]
        public List<ProductImageViewModel>? ExistingImages { get; set; } // 將 ProductImage 轉換為 ViewModel

        // <--- 新增：用於接收被選為主圖的圖片 ID
        [Display(Name = "主圖")]
        public int? MainImageId { get; set; } // 新增這個屬性

        // 新圖片上傳
        [Display(Name = "新增商品圖片")]
        [DataType(DataType.Upload)]
        public List<IFormFile>? NewImageFiles { get; set; } // 允許上傳多個新圖片文件
    }

    // 用於展示現有圖片的子 ViewModel
    public class ProductImageViewModel
    {
        public int Id { get; set; } // ProductImage 的 ID
        public string ImagePath { get; set; }
        public int SortOrder { get; set; }

        // 為了在 View 中方便操作，例如刪除或設定主圖
        public bool IsMarkedForDeletion { get; set; } // 用於標記要刪除的圖片
    }
}