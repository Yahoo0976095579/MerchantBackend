using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // 用於 SelectListItem

namespace MerchantBackend.Models
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "顯示名稱為必填項。")] // <--- 新增
        [Display(Name = "顯示名稱")] // <--- 新增
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "電子郵件為必填項。")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址。")]
        [Display(Name = "電子郵件 (使用者名稱)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密碼為必填項。")]
        [StringLength(100, ErrorMessage = "{0} 長度必須至少為 {2} 個字元。", MinimumLength = 6)] // Identity 預設要求6個字元
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不相符。")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "角色")]
        public List<string> SelectedRoles { get; set; } = new List<string>(); // 用於接收選定的角色

        public IEnumerable<SelectListItem>? AvailableRoles { get; set; } // 用於顯示所有可選角色
    }
}