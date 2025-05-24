using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MerchantBackend.Services;
using MerchantBackend.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // 這個 using Microsoft.AspNetCore.Mvc.Rendering; 應該不需要在 UserController 這裡，可以移除
using MerchantBackend.Data;
using System.Collections.Generic;
using System;

namespace MerchantBackend.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditService _auditService;
        private readonly ApplicationDbContext _context;

        public UserController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuditService auditService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditService = auditService;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPage"] = pageNumber;

            var usersQuery = _userManager.Users
                .GroupJoin(
                    _context.UserProfiles,
                    user => user.Id,
                    profile => profile.UserId,
                    (user, profiles) => new { User = user, Profile = profiles.FirstOrDefault() }
                )
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(x =>
                    (x.User.Email != null && x.User.Email.Contains(searchString)) ||
                    (x.Profile != null && x.Profile.DisplayName.Contains(searchString))
                );
            }

            var totalUsers = await usersQuery.CountAsync();
            var paginatedUsers = await usersQuery
                .OrderBy(x => x.User.Email)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userViewModels = new List<UserViewModel>();
            foreach (var item in paginatedUsers)
            {
                var roles = await _userManager.GetRolesAsync(item.User);
                userViewModels.Add(new UserViewModel
                {
                    Id = item.User.Id,
                    DisplayName = item.Profile?.DisplayName,
                    Email = item.User.Email,
                    Roles = string.Join(", ", roles),
                    LockoutEnd = item.User.LockoutEnd,
                    LockoutEnabled = item.User.LockoutEnabled,
                });
            }

            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            ViewBag.PageSize = pageSize;

            return View(userViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new UserCreateViewModel
            {
                AvailableRoles = roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "此電子郵件已被註冊。");
                    model.AvailableRoles = (await _roleManager.Roles.ToListAsync()).Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
                    return View(model);
                }

                var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        var rolesToAdd = model.SelectedRoles.Intersect(new[] { "Manager", "Editor", "User" }).ToList();
                        await _userManager.AddToRolesAsync(user, rolesToAdd);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    var userProfile = new UserProfile
                    {
                        UserId = user.Id,
                        DisplayName = model.DisplayName
                    };
                    await _context.UserProfiles.AddAsync(userProfile);
                    await _context.SaveChangesAsync();

                    // <--- 修正點 1：在 details 中包含 DisplayName
                    await _auditService.LogAsync(
                        actionType: "User_Created",
                        targetEntity: "User",
                        targetEntityId: user.Id,
                        user: User,
                        details: new { user.Email, DisplayName = userProfile.DisplayName, Roles = model.SelectedRoles }, // <--- 修改這裡
                        outcome: "Success"
                    );

                    TempData["SuccessMessage"] = $"使用者 {userProfile.DisplayName} ({user.Email}) 創建成功！"; // 修正訊息顯示
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AvailableRoles = (await _roleManager.Roles.ToListAsync()).Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles
                                    .Include(up => up.User)
                                    .FirstOrDefaultAsync(up => up.UserId == id);

            if (userProfile == null || userProfile.User == null)
            {
                return NotFound();
            }

            var user = userProfile.User;

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserEditViewModel
            {
                Id = user.Id,
                DisplayName = userProfile.DisplayName,
                Email = user.Email,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                SelectedRoles = userRoles.ToList(),
                AvailableRoles = allRoles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == user.Id);
                if (userProfile == null)
                {
                    userProfile = new UserProfile { UserId = user.Id, DisplayName = model.DisplayName };
                    await _context.UserProfiles.AddAsync(userProfile);
                    // 注意：這裡只 Add，還沒 SaveChangesAsync
                }

                var originalUser = new
                {
                    Email = user.Email,
                    DisplayName = userProfile.DisplayName,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    Roles = await _userManager.GetRolesAsync(user)
                };

                userProfile.DisplayName = model.DisplayName;

                if (user.Email != model.Email)
                {
                    var existingUserWithEmail = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
                    {
                        ModelState.AddModelError("Email", "此電子郵件已被其他使用者使用。");
                        model.AvailableRoles = (await _roleManager.Roles.ToListAsync()).Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
                        return View(model);
                    }
                    user.Email = model.Email;
                    user.NormalizedEmail = _userManager.NormalizeEmail(model.Email);
                    user.UserName = model.Email;
                    user.NormalizedUserName = _userManager.NormalizeName(model.Email);
                }

                user.LockoutEnabled = model.LockoutEnabled;
                user.LockoutEnd = model.LockoutEnabled ? model.LockoutEnd : null;

                var updateResult = await _userManager.UpdateAsync(user); // 更新 IdentityUser

                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    model.AvailableRoles = (await _roleManager.Roles.ToListAsync()).Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
                    return View(model);
                }

                // <--- 關鍵點：在更新 IdentityUser 後，也要保存 UserProfile 的變更
                await _context.SaveChangesAsync();


                var userRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = userRoles.Except(model.SelectedRoles).ToList();
                var rolesToAdd = model.SelectedRoles.Except(userRoles).ToList();

                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                await _userManager.AddToRolesAsync(user, rolesToAdd);

                // <--- 修正點 2：使用 ResetPasswordAsync
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    // 生成密碼重設 Token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    if (!resetPasswordResult.Succeeded)
                    {
                        foreach (var error in resetPasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, $"密碼重設失敗: {error.Description}");
                        }
                        model.AvailableRoles = (await _roleManager.Roles.ToListAsync()).Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
                        return View(model);
                    }
                }

                var updatedUser = new
                {
                    Email = user.Email,
                    DisplayName = userProfile.DisplayName, // 記錄更新後 DisplayName
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    Roles = await _userManager.GetRolesAsync(user)
                };

                await _auditService.LogAsync(
                    actionType: "User_Edited",
                    targetEntity: "User",
                    targetEntityId: user.Id,
                    user: User,
                    details: new { Original = originalUser, Updated = updatedUser },
                    outcome: "Success"
                );

                TempData["SuccessMessage"] = $"使用者 {userProfile.DisplayName} ({user.Email}) 更新成功！";
                return RedirectToAction(nameof(Index));
            }


            model.AvailableRoles = (await _roleManager.Roles.ToListAsync()).Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var userProfile = await _context.UserProfiles
                                    .Include(up => up.User) // 從 UserProfile 包含 IdentityUser
                                    .FirstOrDefaultAsync(up => up.UserId == id);

            if (userProfile == null || userProfile.User == null)
            {
                return NotFound();
            }

            var user = userProfile.User; // 獲取 IdentityUser
            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserViewModel
            {
                Id = user.Id,
                DisplayName = userProfile.DisplayName, // 從 UserProfile 獲取
                Email = user.Email,
                Roles = string.Join(", ", roles),
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
            };
            return View(model);
        }
    }
}