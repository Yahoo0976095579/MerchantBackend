using System;
using System.ComponentModel.DataAnnotations;

namespace MerchantBackend.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "操作時間")] // <--- 中文化
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // 記錄操作發生的時間 (UTC)

        [Required]
        [MaxLength(450)] // IdentityUser Id 通常是 GUID，長度較長
        public string ActorId { get; set; } // 執行操作的使用者 Id

        [Required]
        [Display(Name = "操作者")] // <--- 中文化
        [MaxLength(256)]
        public string ActorUsername { get; set; } // 執行操作的使用者名稱

        [Required]
        [Display(Name = "操作類型")] // <--- 中文化
        [MaxLength(100)]
        public string ActionType { get; set; } // 操作類型，例如 "UserCreated", "ProductEdited"

        [Required]
        [Display(Name = "目標")] // <--- 中文化
        [MaxLength(100)]
        public string TargetEntity { get; set; } // 受操作影響的實體類型，例如 "User", "Product"

        [MaxLength(450)] // 目標實體 Id，可能為 null 或不同類型
        [Display(Name = "目標ID")] // <--- 中文化
        public string? TargetEntityId { get; set; } // 受操作影響的實體的主鍵 (可空)
        [Display(Name = "詳情")] // <--- 中文化
        public string? Details { get; set; } // 操作的具體內容或變更 (建議以 JSON 格式儲存)
        [Display(Name = "結果")] // <--- 中文化
        [MaxLength(50)]
        public string? Outcome { get; set; } // 操作結果，例如 "Success", "Failure"
    }
}