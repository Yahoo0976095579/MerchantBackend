// Models/Products/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // 用於 [Column]
using System.Collections.Generic; // 用於導航屬性
using MerchantBackend.Models;
namespace MerchantBackend.Models.Products
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "商品名稱為必填項。")]
        [MaxLength(256)]
        [Display(Name = "商品名稱")]
        public string Name { get; set; }

        [MaxLength(1000)]
        [Display(Name = "商品描述")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "價格為必填項。")]
        [Range(0.01, double.MaxValue, ErrorMessage = "價格必須大於零。")]
        [Column(TypeName = "decimal(18,2)")] // 精確到小數點後兩位
        [Display(Name = "價格")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "庫存為必填項。")]
        [Range(0, int.MaxValue, ErrorMessage = "庫存不能為負數。")]
        [Display(Name = "庫存")]
        public int Stock { get; set; }

        [Display(Name = "是否上架")]
        public bool IsActive { get; set; } = true; // 預設上架

        [Display(Name = "是否已刪除 (邏輯刪除)")]
        public bool IsDeleted { get; set; } = false; // 預設未刪除

        [Display(Name = "建立時間")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "更新時間")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // 外鍵關聯到 Category (一個商品只能有一個分類，如果有多個則需要中間表)
        [Display(Name = "分類")]
        public int? CategoryId { get; set; } // 可空，允許商品沒有分類

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; } // 導航屬性

        // 導航屬性：商品圖片 (一個商品有多張圖片)
        public ICollection<ProductImage>? Images { get; set; }

        // 導航屬性：商品標籤 (多對多關係，需要 ProductTag 中間表)
        public ICollection<ProductTag>? ProductTags { get; set; }
    }
}