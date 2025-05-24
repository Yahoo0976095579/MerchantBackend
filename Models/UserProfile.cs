using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // 引入 IdentityUser

namespace MerchantBackend.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)] // IdentityUser.Id 的長度通常是 450
        public string UserId { get; set; } // 關聯到 AspNetUsers 的外鍵

        [Required(ErrorMessage = "顯示名稱為必填項。")]
        [MaxLength(256)]
        [Display(Name = "顯示名稱")]
        public string DisplayName { get; set; } // 用於顯示在介面上的中文名稱

        // Navigation property to IdentityUser (可選，但推薦建立關聯)
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } // <--- 確認這裡使用 IdentityUser
    }
}