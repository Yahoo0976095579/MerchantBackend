// Models/Tag.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // 用於導航屬性

namespace MerchantBackend.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "標籤名稱為必填項。")]
        [MaxLength(50)]
        [Display(Name = "標籤名稱")]
        public string Name { get; set; }

        // Navigation property for products (如果一個商品有多個標籤，需要中間表)
        // public ICollection<ProductTag>? ProductTags { get; set; }
    }
}