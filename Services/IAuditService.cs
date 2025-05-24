using System.Threading.Tasks;
using System.Security.Claims; // 用於獲取當前使用者資訊
using Newtonsoft.Json; // 用於序列化 Details

namespace MerchantBackend.Services
{
    public interface IAuditService
    {
        /// <summary>
        /// 記錄稽核日誌。
        /// </summary>
        /// <param name="actionType">操作類型，例如 "UserCreated", "ProductEdited"。</param>
        /// <param name="targetEntity">受操作影響的實體類型，例如 "User", "Product"。</param>
        /// <param name="targetEntityId">受操作影響的實體主鍵 ID。</param>
        /// <param name="user">執行操作的 ClaimsPrincipal (通常是 HttpContext.User)。</param>
        /// <param name="details">操作的詳細內容，會被序列化為 JSON 儲存。</param>
        /// <param name="outcome">操作結果，例如 "Success", "Failure"。</param>
        Task LogAsync(
            string actionType,
            string targetEntity,
            string? targetEntityId,
            ClaimsPrincipal user,
            object? details = null,
            string outcome = "Success"
        );

        /// <summary>
        /// 記錄匿名操作或系統操作的稽核日誌。
        /// </summary>
        /// <param name="actionType">操作類型。</param>
        /// <param name="targetEntity">受操作影響的實體類型。</param>
        /// <param name="targetEntityId">受操作影響的實體主鍵 ID。</param>
        /// <param name="actorId">執行操作的特定 Id (例如系統服務 Id)，如果為空則為 "System"。</param>
        /// <param name="actorUsername">執行操作的特定名稱 (例如 "System")，如果為空則為 "System"。</param>
        /// <param name="details">操作的詳細內容。</param>
        /// <param name="outcome">操作結果。</param>
        Task LogAnonymousAsync(
            string actionType,
            string targetEntity,
            string? targetEntityId,
            string? actorId = "System",
            string? actorUsername = "System",
            object? details = null,
            string outcome = "Success"
        );
    }
}