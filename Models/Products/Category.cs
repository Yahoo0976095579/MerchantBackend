// Models/Category.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // 用於導航屬性

namespace MerchantBackend.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "分類名稱為必填項。")]
        [MaxLength(100)]
        [Display(Name = "分類名稱")]
        public string Name { get; set; }

        [MaxLength(500)]
        [Display(Name = "分類描述")]
        public string? Description { get; set; }

        // Navigation property for products (如果一個商品只屬於一個分類)
        // public ICollection<Product>? Products { get; set; } // 如果是多對多，需要中間表
    }
}