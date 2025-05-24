// Models/Products/ProductImage.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MerchantBackend.Models.Products
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; } // 外鍵關聯到 Product

        [Required(ErrorMessage = "圖片路徑為必填項。")]
        [MaxLength(500)]
        [Display(Name = "圖片路徑")]
        public string ImagePath { get; set; } // 儲存圖片的相對路徑或 URL

        [Display(Name = "是否為主圖")]
        public bool IsMain { get; set; } // 標識是否為商品的主圖片

        [Display(Name = "排序")]
        public int SortOrder { get; set; } // 圖片顯示順序

        [ForeignKey("ProductId")]
        public Product Product { get; set; } // 導航屬性
    }
}