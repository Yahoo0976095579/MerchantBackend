using System;
using System.ComponentModel.DataAnnotations;

namespace MerchantBackend.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "顯示名稱")] // 用於列表顯示
        public string? DisplayName { get; set; } // <--- 新增：用於顯示名稱

        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Display(Name = "角色")]
        public string Roles { get; set; } // 用於顯示角色的字串

        [Display(Name = "鎖定結束時間")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [Display(Name = "鎖定狀態")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "帳號狀態")]
        public string AccountStatus
        {
            get
            {
                if (LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow)
                {
                    return "已鎖定";
                }
                else if (!LockoutEnabled) // 如果沒有啟用鎖定功能，通常表示正常
                {
                    return "正常";
                }
                return "正常"; // 預設正常，或可加入更多狀態判斷
            }
        }
    }
}