using Microsoft.AspNetCore.Identity;
using MerchantBackend.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using MerchantBackend.Models;
using Microsoft.EntityFrameworkCore; // 引入 UserProfile 模型

namespace MerchantBackend.SeedData
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 獲取 ApplicationDbContext 實例
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // <--- 新增這行
            // 定義角色名稱
            string[] roleNames = { "Manager", "Editor", "User" };

            // 創建角色
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 創建 Manager 帳號
            string managerUserEmail = "yahoo0976095579@gmail.com"; // 設定你的 Manager 電子郵件
            string managerPassword = "Mm@102050"; // 設定你的 Manager 密碼，必須符合 Identity 策略
            string managerDisplayName = "系統管理員"; // <--- 定義 Manager 的顯示名稱

            if (await userManager.FindByEmailAsync(managerUserEmail) == null)
            {
                var managerUser = new IdentityUser
                {
                    UserName = managerUserEmail,
                    Email = managerUserEmail,
                    EmailConfirmed = true // 開發環境方便，直接確認電子郵件
                };

                var result = await userManager.CreateAsync(managerUser, managerPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                    // 如果你希望這個 Manager 同時是 Editor 也可以加
                    await userManager.AddToRoleAsync(managerUser, "Editor");

                    // <--- 新增：為 Manager 創建 UserProfile 記錄
                    var managerUserProfile = new UserProfile
                    {
                        UserId = managerUser.Id,
                        DisplayName = managerDisplayName
                    };
                    await context.UserProfiles.AddAsync(managerUserProfile);
                    await context.SaveChangesAsync(); // 保存 UserProfile 數據
                }
                else
                {
                    // 輸出錯誤訊息，幫助調試
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating manager user: {error.Description}");
                    }
                }
            }
            // 處理現有使用者沒有 DisplayName 的情況（在第一次運行時，如果之前已經有用戶）
            // 檢查所有沒有對應 UserProfile 的 IdentityUser
            var existingUsersWithoutProfile = await userManager.Users
                .Where(u => !context.UserProfiles.Any(up => up.UserId == u.Id)).ToListAsync();


            foreach (var user in existingUsersWithoutProfile)
            {
                var defaultDisplayName = user.UserName; // 可以使用 UserName 作為預設顯示名稱
                var userProfile = new UserProfile
                {
                    UserId = user.Id,
                    DisplayName = defaultDisplayName
                };
                await context.UserProfiles.AddAsync(userProfile);
            }
            await context.SaveChangesAsync(); // 保存新創建的 UserProfile 數據
        }
    }
}