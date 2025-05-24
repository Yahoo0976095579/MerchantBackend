// Models/ProductListViewModel.cs
using System.ComponentModel.DataAnnotations;
using System;

namespace MerchantBackend.Models
{
    public class ProductListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "商品名稱")]
        public string Name { get; set; }

        // <--- 新增：商品描述
        [Display(Name = "商品描述")]
        public string? Description { get; set; }

        [Display(Name = "價格")]
        [DisplayFormat(DataFormatString = "{0:C}")] // 顯示貨幣格式
        public decimal Price { get; set; }

        [Display(Name = "庫存")]
        public int Stock { get; set; }

        [Display(Name = "是否上架")]
        public bool IsActive { get; set; }

        [Display(Name = "分類")]
        public string? CategoryName { get; set; } // 用於顯示分類名稱

        [Display(Name = "標籤")]
        public string? Tags { get; set; } // 用於顯示標籤名稱

        [Display(Name = "主圖")]
        public string? MainImagePath { get; set; } // 用於顯示主圖片縮略圖
    }
}