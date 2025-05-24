# 商家後台管理系統 (Merchant Backend Management System)

![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)
![SQL Server](https://img.shields.io/badge/SQL_Server-grey.svg)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core_MVC-purple.svg)
![MIT License](https://img.shields.io/badge/License-MIT-green.svg)

## 專案簡介

這是一個使用 **ASP.NET Core 8 MVC** 開發的綜合性後台管理系統，旨在為電子商務平台提供使用者、商品、分類和標籤的管理功能。專案設計著重於**安全性**、**可稽核性 (Auditability)** 和管理員操作的**便利性**。

系統採用**邏輯刪除 (Soft Delete)** 策略來保留歷史數據，並通過詳盡的**稽核日誌**記錄所有關鍵操作，確保數據的可追溯性。

---

## 主要功能 🚀

* **安全認證與授權**
    * 基於 **ASP.NET Core Identity** 的身份驗證和角色管理。
    * 後台登入僅限 `Manager` (管理員) 和 `Editor` (編輯者) 角色。
    * 精細的角色權限控制 (`Manager` 擁有最高權限，`Editor` 專注於商品相關管理)。
    * 所有密碼錯誤訊息已**中文化**。

* **後台儀表板** 📊
    * 提供登入後直觀的概覽頁面，顯示總使用者數、總商品數、上架中商品數、總分類數、總標籤數和今日登入次數等關鍵統計數據。

* **後台使用者管理 (僅限 `Manager`)** 👥
    * **使用者列表：** 顯示所有後台使用者 (IdentityUser) 的列表，包含顯示名稱、電子郵件、角色和帳戶狀態。
    * **新增使用者：** `Manager` 可以為新加入的後台成員創建帳號，設定初始密碼，並指派 `Manager`、`Editor` 或 `User` 角色。
    * **編輯使用者：** `Manager` 可以修改使用者顯示名稱、電子郵件、帳戶鎖定/解鎖狀態，並調整其角色。同時提供直接重設密碼功能。
    * **獨立顯示名稱：** 使用者介面顯示的名稱 (`DisplayName`) 儲存在獨立的 `UserProfile` 資料表中，不影響 Identity 核心的 `UserName` (與 Email 保持一致用於登入)。

* **個人帳戶管理 (`Manager` 與 `Editor`)** 👤
    * 已登入的後台使用者可以查看自己的基本資訊。
    * 安全修改自己的密碼，並有成功/失敗訊息提示。

* **商品分類管理 (`Manager` 完整 CRUD，`Editor` 查看)** 📂
    * 對商品的分類進行建立、讀取、更新、刪除 (CRUD) 操作。
    * 包含分類名稱和描述。

* **商品標籤管理 (`Manager` 完整 CRUD，`Editor` 查看)** 🏷️
    * 對商品的標籤進行建立、讀取、更新、刪除 (CRUD) 操作。
    * 包含標籤名稱。

* **商品管理 (`Manager` 與 `Editor` 完整 CRUD)** 🛍️
    * **商品列表：** 顯示所有未邏輯刪除的商品，包含名稱、價格、庫存、上架狀態、主圖、所屬分類和標籤。支援搜尋、分類/標籤篩選和上架狀態篩選。
    * **新增商品：** 允許新增商品，包含基本屬性、選擇分類和多個標籤，並支援上傳多張圖片並設定主圖。
    * **編輯商品：** 全面編輯商品資訊，包括管理現有圖片 (刪除、設定主圖)、上傳新圖片，以及修改分類和標籤。
    * **邏輯刪除商品：** 將商品標記為 `IsDeleted = true`，而非物理刪除，以保留數據和追溯性。

* **稽核日誌** 📜
    * 詳細記錄所有重要的後台操作 (使用者創建/編輯、商品 CRUD、分類 CRUD、標籤 CRUD、登入/登出、密碼修改等)。
    * 提供網頁介面供 `Manager` 角色查看、搜尋、篩選 (依操作類型、目標實體、時間範圍) 和分頁瀏覽日誌。
    * 所有操作類型已**中文化**顯示。

* **使用者介面與體驗** ✨
    * 採用 **Bootstrap 5** 構建，提供響應式設計，適應不同設備尺寸。
    * 所有表單均具備客戶端和伺服器端驗證。
    * 統一的操作成功與失敗訊息提示。

---

## 技術棧 🛠️

* **開發框架：** ASP.NET Core 8 (MVC)
* **數據庫：** SQL Server (使用 Entity Framework Core)
* **身份驗證與授權：** ASP.NET Core Identity
* **前端 UI：** Bootstrap 5
* **其他庫：**
    * jQuery & jQuery Validation (用於客戶端表單驗證)
    * Newtonsoft.Json (用於稽核日誌中的詳細資訊序列化)

---

## 開始使用 🏁

### 先決條件

在本地運行此專案，您需要安裝：

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (或 SQL Server LocalDB/Express)
* (可選) [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/zh-tw/sql/ssms/download-sql-server-management-studio-ssms) 或 [Azure Data Studio](https://docs.microsoft.com/zh-tw/sql/azure-data-studio/) 以管理資料庫

### 1. 克隆儲存庫

```bash
git clone [https://github.com/YOUR_GITHUB_USERNAME/MerchantBackend.git](https://github.com/YOUR_GITHUB_USERNAME/MerchantBackend.git)
cd MerchantBackend
將 YOUR_GITHUB_USERNAME 替換為您的 GitHub 使用者名稱。

### 2. 資料庫設定
###配置連接字串：
開啟 appsettings.json 檔案，在 ConnectionStrings 區塊中，將 YOUR_SERVER_NAME 替換為您的 SQL Server 實例名稱。

JSON

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MerchantBackendDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "FileUploadSettings": {
    "ProductImageUploadFolder": "images/products" // 商品圖片儲存路徑
  }
}
提示： 如果使用 SQL Server 驗證，請將 Trusted_Connection=True 替換為 User ID=YourUser;Password=YourPassword;。

執行資料庫遷移：
在專案的根目錄下 (即 MerchantBackend.csproj 所在資料夾)，執行以下命令來創建所有資料庫表和關聯：

Bash

dotnet ef database update
這會自動執行所有待處理的遷移 (InitialIdentitySetup, AddAuditLogTable, AddUserProfileTable, AddCategoriesAndTagsTables, AddProductsAndRelations)。

### 3. 種子初始數據 🌱
專案啟動時，會自動運行 DbInitializer 來創建預設的角色 (Manager, Editor, User) 和一個初始的 Manager 帳號。

重要： 初始 Manager 帳號的電子郵件是 admin@example.com。

請修改 SeedData/DbInitializer.cs 中的 managerPassword 變數，設定一個符合 Identity 策略的強密碼，並記住它。

C#

// SeedData/DbInitializer.cs 中的片段
string managerUserEmail = "admin@example.com";
string managerPassword = "YourSecurePassword123!"; // <-- 請務必修改此密碼！
### 4. 運行應用程式 ▶️
在專案的根目錄下，執行以下命令：

Bash

dotnet run
應用程式將會在 https://localhost:PORT (具體端口號會顯示在終端機中) 啟動。

使用指南 📖
訪問後台：
在瀏覽器中訪問 https://localhost:PORT。如果未登入，您將被自動重定向到登入頁面。

登入：
使用您在 DbInitializer.cs 中設定的 Manager 帳號 (預設 admin@example.com) 和密碼登入。

角色與權限：
Manager (管理員)： 擁有所有後台功能的完整權限 (使用者管理、商品 CRUD、分類 CRUD、標籤 CRUD、稽核日誌查看、個人帳戶管理)。
Editor (編輯者)： 擁有商品管理、分類查看、標籤查看、個人帳戶管理的權限。無法訪問使用者管理和稽核日誌。
User (一般使用者)： 無法登入此後台系統。
專案結構概覽 📁
Controllers/: ASP.NET Core MVC 控制器，處理 HTTP 請求和業務邏輯。
Models/: 包含所有 ViewModels (例如 UserViewModel, ProductCreateViewModel 等) 和一些通用實體 (AuditLog, UserProfile)。
Models/Products/: 包含商品相關的實體模型 (Product, ProductImage, Category, Tag, ProductTag)。
Data/: 包含資料庫上下文 (ApplicationDbContext) 和 Entity Framework Core 遷移檔案。
Services/: 包含應用程式服務，例如 IAuditService 和 AuditService。
Identity/: 包含擴充的 ApplicationUser (如果未來決定擴充 IdentityUser)。
IdentityLocalizations/: 包含自定義的 IdentityErrorDescriber，用於中文化 Identity 的錯誤訊息。
SeedData/: 包含 DbInitializer 類別，用於資料庫的初始數據種子。
Views/: 包含 Razor View 檔案，用於渲染使用者介面。
wwwroot/: 靜態檔案存放處 (CSS, JavaScript, 圖片，例如 images/products)。
appsettings.json: 應用程式配置檔案 (資料庫連接字串、檔案上傳路徑等)。
未來可能的擴展 (Roadmap) 🗺️
使用者瀏覽網站 (電商前台)：
基於 Web API： 建立獨立的 ASP.NET Core Web API 專案，提供資料給前端框架 (React, Angular, Vue.js 等) 構建的電商前台。
JWT 身份驗證： 為前端 API 實現基於 JWT 的身份驗證和授權。
電子郵件驗證與通知：
為使用者註冊、密碼重設、電子郵件變更等操作集成電子郵件發送功能。
細粒度權限管理：
基於聲明 (Claims) 或策略 (Policies) 實現更細緻的權限控制。
商品多變數/SKU 管理：
支援不同顏色、尺寸等商品變數的庫存管理。
訂單管理系統：
添加訂單創建、處理、追蹤、退款等功能。
客戶服務模組：
管理客戶諮詢、投訴等。
報表與數據分析：
基於業務數據生成圖表和報告。
部署腳本：
為應用程式提供自動化部署到 IIS、Azure 或 Docker 等平台的腳本。
許可證 ⚖️
此專案在 MIT 許可證下發布。詳情請參閱 LICENSE 檔案。