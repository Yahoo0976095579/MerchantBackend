using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MerchantBackend.Models; // 假設您會創建 LoginViewModel
using MerchantBackend.Services; // 引入 IAuditService
using MerchantBackend.Data; // 引入 ApplicationDbContext
using Microsoft.EntityFrameworkCore; // 引入 Include
using MerchantBackend.Data; // 引入 ApplicationDbContext
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // 引入 Include

namespace MerchantBackend.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager; // 即使 V1 不用，先引入方便之後擴充
        private readonly IAuditService _auditService; // 添加 AuditService 注入
        private readonly ApplicationDbContext _context; // <--- 新增：注入 DbContext


        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IAuditService auditService, ApplicationDbContext context) // 添加到建構子參數
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auditService = auditService; // 賦值
            _context=context;

        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous] // 允許未登入者訪問
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // 獲取 IdentityUser 並包含 UserProfile
                    // <--- 關鍵修改：獲取 IdentityUser 和 UserProfile
                    var user = await _userManager.FindByEmailAsync(model.Email); // 先獲取 IdentityUser
                    var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == user.Id); // 再獲取對應的 UserProfile

                    if (user != null && (await _userManager.IsInRoleAsync(user, "Manager") || await _userManager.IsInRoleAsync(user, "Editor")))
                    {
                        await _auditService.LogAsync(
                            actionType: "Login_Success",
                            targetEntity: "User",
                            targetEntityId: user.Id,
                            user: User,
                            details: new { Email = model.Email, DisplayName = userProfile?.DisplayName }, // <--- 從 UserProfile 獲取 DisplayName
                            outcome: "Success"
                        );
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        await _signInManager.SignOutAsync(); // 先登出無權限的使用者

                        // 獲取使用者 ID 以便日誌記錄，即使角色不符
                        var userId = user?.Id;
                        var userDisplayName = userProfile?.DisplayName; // 嘗試獲取 DisplayName

                        await _auditService.LogAnonymousAsync(
                            actionType: "Login_Failure_UnauthorizedRole",
                            targetEntity: "User",
                            targetEntityId: userId,
                            actorUsername: userDisplayName ?? model.Email, // 優先使用 DisplayName，否則用 Email
                            details: new { Email = model.Email, DisplayName = userDisplayName, Reason = "Unauthorized role for backend access" },
                            outcome: "Failure"
                        );
                        ModelState.AddModelError(string.Empty, "您沒有權限登入此後台系統。");
                        return View(model);
                    }
                }
                else if (result.IsLockedOut)
                {
                    // 帳號鎖定，記錄稽核日誌
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    await _auditService.LogAnonymousAsync(
                        actionType: "Login_Failure_LockedOut",
                        targetEntity: "User",
                        targetEntityId: user?.Id,
                        actorUsername: model.Email,
                        details: new { Email = model.Email, Reason = "Account is locked out" },
                        outcome: "Failure"
                    );
                    ModelState.AddModelError(string.Empty, "您的帳號已被鎖定。");
                    return View(model);
                }
                else
                {
                    // 其他登入失敗 (例如密碼錯誤)，記錄稽核日誌
                    // 注意：這裡無法直接拿到 UserId，因為登入可能失敗在 FindByEmail 之前，或密碼錯誤
                    // 所以用 LogAnonymousAsync 記錄嘗試的 Email
                    await _auditService.LogAnonymousAsync(
                        actionType: "Login_Failure_InvalidCredentials",
                        targetEntity: "User",
                        targetEntityId: null, // 無法確定具體使用者 ID
                        actorUsername: model.Email,
                        details: new { Email = model.Email, Reason = "Invalid credentials" },
                        outcome: "Failure"
                    );
                    ModelState.AddModelError(string.Empty, "登入失敗，請檢查您的電子郵件或密碼。");
                    return View(model);
                }
            }
            // 如果模型驗證失敗，不記錄日誌，因為還未嘗試登入
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // 獲取當前登入使用者的 IdentityUser ID
            var currentUserId = _userManager.GetUserId(User);
            // 獲取對應的 UserProfile 以取得 DisplayName
            var currentUserProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == currentUserId);
            var userNameForLog = currentUserProfile?.DisplayName ?? User.Identity?.Name ?? currentUserId; // 優先使用 DisplayName

            // 登出前記錄稽核日誌
            // 注意：Logout Action 執行時，使用者通常還是登入狀態，所以可以使用 User Principal
            await _auditService.LogAsync(
                actionType: "Logout_Success",
                targetEntity: "User",
                targetEntityId: currentUserId,
                user: User,
                details: new { DisplayName = currentUserProfile?.DisplayName, Email = User.Identity?.Name }, // <--- 從 UserProfile 獲取 DisplayName
                outcome: "Success"
            );

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Manager, Editor")] // 只有 Manager 和 Editor 角色可以訪問
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User); // 獲取當前登入的 IdentityUser
            if (user == null)
            {
                return NotFound($"無法載入使用者 ID: '{_userManager.GetUserId(User)}'.");
            }

            // 獲取使用者的 UserProfile
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == user.Id);

            var model = new MyProfileViewModel
            {
                Email = user.Email,
                DisplayName = userProfile?.DisplayName // 從 UserProfile 獲取顯示名稱
            };

            return View(model);
        }

        [Authorize(Roles = "Manager, Editor")] // 只有 Manager 和 Editor 角色可以訪問
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(MyProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User); // 獲取當前登入的 IdentityUser
            if (user == null)
            {
                return NotFound($"無法載入使用者 ID: '{_userManager.GetUserId(User)}'.");
            }

            // 在驗證模型時，排除 DisplayName 和 Email 的 Required 驗證，因為它們不在此處修改
            // (通常這些欄位在 GET 時顯示，POST 時不需要提交回來驗證)
            ModelState.Remove(nameof(model.Email));
            ModelState.Remove(nameof(model.DisplayName));

            if (!ModelState.IsValid)
            {
                // 如果模型驗證失敗，重新載入 DisplayName 和 Email 並返回 View
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == user.Id);
                model.Email = user.Email;
                model.DisplayName = userProfile?.DisplayName;
                return View(model);
            }

            // 驗證當前密碼是否正確並修改密碼
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                // 如果修改失敗，重新載入 DisplayName 和 Email 並返回 View
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == user.Id);
                model.Email = user.Email;
                model.DisplayName = userProfile?.DisplayName;
                return View(model);
            }

            // 記錄稽核日誌
            var userProfileForLog = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == user.Id);
            await _auditService.LogAsync(
                actionType: "User_PasswordChanged", // 新增的操作類型
                targetEntity: "User",
                targetEntityId: user.Id,
                user: User,
                details: new { Email = user.Email, DisplayName = userProfileForLog?.DisplayName, Outcome = "Password changed" },
                outcome: "Success"
            );

            TempData["SuccessMessage"] = "您的密碼已成功修改。";
            return RedirectToAction(nameof(MyProfile)); // 重定向回個人頁面
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home"); // 預設重定向到後台首頁
            }
        }


        
    }
}