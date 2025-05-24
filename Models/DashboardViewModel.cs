// Models/DashboardViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace MerchantBackend.Models
{
    public class DashboardViewModel
    {
        [Display(Name = "總使用者數")]
        public int TotalUsers { get; set; }

        [Display(Name = "總商品數")]
        public int TotalProducts { get; set; }

        [Display(Name = "上架中商品數")]
        public int ActiveProducts { get; set; }

        [Display(Name = "總分類數")]
        public int TotalCategories { get; set; }

        [Display(Name = "總標籤數")]
        public int TotalTags { get; set; }

        [Display(Name = "今日登入次數")]
        public int TodayLogins { get; set; }

        // 可以添加更多未來可能會有的數據
        // [Display(Name = "待處理訂單")]
        // public int PendingOrders { get; set; }

        // [Display(Name = "總銷售額")]
        // public decimal TotalSales { get; set; }
    }
}