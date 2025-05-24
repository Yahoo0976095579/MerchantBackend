using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MerchantBackend.Models
{
    public class UserEditViewModel
    {
        public string Id { get; set; } // 使用者 ID

        [Required(ErrorMessage = "顯示名稱為必填項。")] // <--- 新增
        [Display(Name = "顯示名稱")] // <--- 新增
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "電子郵件為必填項。")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址。")]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Display(Name = "鎖定帳號直到")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset? LockoutEnd { get; set; } // 帳號鎖定結束時間

        [Display(Name = "啟用帳號鎖定")]
        public bool LockoutEnabled { get; set; } // 是否啟用帳號鎖定功能

        [Display(Name = "角色")]
        public List<string> SelectedRoles { get; set; } = new List<string>(); // 用於接收選定的角色

        public IEnumerable<SelectListItem>? AvailableRoles { get; set; } // 用於顯示所有可選角色

        // 密碼重設相關欄位 (可選填)
        [DataType(DataType.Password)]
        [Display(Name = "新密碼 (不修改請留空)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認新密碼不相符。")]
        public string? ConfirmNewPassword { get; set; }
    }
}