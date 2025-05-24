// Models/ProductCreateViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // 用於 IFormFile
using Microsoft.AspNetCore.Mvc.Rendering; // 用於 SelectListItem

namespace MerchantBackend.Models
{
    public class ProductCreateViewModel
    {
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
        public bool IsActive { get; set; } = true;

        // 分類選擇
        [Display(Name = "分類")]
        public int? SelectedCategoryId { get; set; }
        public IEnumerable<SelectListItem>? AvailableCategories { get; set; }

        // 標籤選擇 (多選)
        [Display(Name = "標籤")]
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? AvailableTags { get; set; }

        // 圖片上傳
        [Display(Name = "商品圖片")]
        [DataType(DataType.Upload)]
        public List<IFormFile>? ImageFiles { get; set; } // 允許上傳多個圖片文件
    }
}