using System.ComponentModel.DataAnnotations;

namespace MerchantBackend.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "電子郵件為必填項。")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址。")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密碼為必填項。")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "記住我?")]
        public bool RememberMe { get; set; }
    }
}