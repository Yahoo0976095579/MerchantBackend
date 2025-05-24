// Services/AuditService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MerchantBackend.Data;
using MerchantBackend.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MerchantBackend.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AuditService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task LogAsync(
            string actionType,
            string targetEntity,
            string? targetEntityId,
            ClaimsPrincipal user,
            object? details = null,
            string outcome = "Success")
        {
            var userId = _userManager.GetUserId(user);
            // 獲取當前登入使用者的 DisplayName
            var currentUserProfile = await _context.UserProfiles
                                            .FirstOrDefaultAsync(up => up.UserId == userId);
            var actorUsername = currentUserProfile?.DisplayName ?? user.Identity?.Name ?? userId; // 優先使用 DisplayName

            if (string.IsNullOrEmpty(userId))
            {
                await LogAnonymousAsync(actionType, targetEntity, targetEntityId, "Unknown", "Unknown User", details, outcome);
                return;
            }

            var auditLog = new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                ActorId = userId,
                ActorUsername = actorUsername,
                ActionType = actionType,
                TargetEntity = targetEntity,
                TargetEntityId = targetEntityId,
                Details = details != null ? JsonConvert.SerializeObject(details) : null,
                Outcome = outcome
            };

            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        // LogAnonymousAsync 保持不變
        public async Task LogAnonymousAsync(
            string actionType,
            string targetEntity,
            string? targetEntityId,
            string? actorId = "System",
            string? actorUsername = "System",
            object? details = null,
            string outcome = "Success")
        {
            var auditLog = new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                ActorId = actorId ?? "System",
                ActorUsername = actorUsername ?? "System",
                ActionType = actionType,
                TargetEntity = targetEntity,
                TargetEntityId = targetEntityId,
                Details = details != null ? JsonConvert.SerializeObject(details) : null,
                Outcome = outcome
            };

            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}