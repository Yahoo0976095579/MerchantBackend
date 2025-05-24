using Microsoft.AspNetCore.Identity;

namespace MerchantBackend.IdentityLocalizations
{
    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "密碼必須至少包含一個非英數字元（例如:!@#$%^&*）。"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "密碼必須至少包含一個小寫字母（'a'-'z'）。"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "密碼必須至少包含一個大寫字母（'A'-'Z'）。"
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"密碼長度必須至少為 {length} 個字元。"
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "密碼必須至少包含一個數字（'0'-'9'）。"
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"電子郵件 '{email}' 已被使用。"
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = $"電子郵件 '{email}' 格式不正確。"
            };
        }
        // <--- 新增這裡：中文化密碼不匹配的錯誤訊息
        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = nameof(PasswordMismatch),
                Description = "當前密碼不正確。"
            };
        }
        // 您也可以覆寫其他您希望中文化的錯誤訊息，例如：
        // public override IdentityError DuplicateUserName(string userName)
        // {
        //     return new IdentityError
        //     {
        //         Code = nameof(DuplicateUserName),
        //         Description = $"使用者名稱 '{userName}' 已被使用。"
        //     };
        // }
    }
}