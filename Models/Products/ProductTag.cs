// Models/Products/ProductTag.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MerchantBackend.Models.Products
{
    // 商品與標籤的多對多關聯中間表
    public class ProductTag
    {
        [Key]
        public int Id { get; set; } // 可以使用複合主鍵，但簡單起見先用單一主鍵

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int TagId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } // 導航屬性

        [ForeignKey("TagId")]
        public Tag Tag { get; set; } // 導航屬性
    }
}