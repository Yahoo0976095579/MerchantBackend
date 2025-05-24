using System.ComponentModel.DataAnnotations;

namespace MerchantBackend.Models
{
    public class MyProfileViewModel
    {
        [Display(Name = "登入電子郵件")]
        public string Email { get; set; } // 顯示當前電子郵件

        [Display(Name = "顯示名稱")]
        public string? DisplayName { get; set; } // 顯示當前顯示名稱

        // 密碼修改相關欄位
        [DataType(DataType.Password)]
        [Display(Name = "當前密碼")]
        [Required(ErrorMessage = "當前密碼為必填項。")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "新密碼為必填項。")]
        [StringLength(100, ErrorMessage = "{0} 長度必須至少為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認新密碼不相符。")]
        public string ConfirmNewPassword { get; set; }
    }
}