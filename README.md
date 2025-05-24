# �Ӯa��x�޲z�t�� (Merchant Backend Management System)

![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)
![SQL Server](https://img.shields.io/badge/SQL_Server-grey.svg)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core_MVC-purple.svg)
![MIT License](https://img.shields.io/badge/License-MIT-green.svg)

## �M��²��

�o�O�@�Өϥ� ASP.NET Core 8 MVC �}�o����X�ʫ�x�޲z�t�ΡA���b���q�l�Ӱȥ��x���ѨϥΪ̡B�ӫ~�B�����M���Ҫ��޲z�\��C�M�׳]�p�ۭ���w���ʡB�i�]�֩� (Auditability) �M�޲z���ާ@���K�Q�ʡC

�t�αĥ��޿�R�� (Soft Delete) �����ӫO�d���v�ƾڡA�óq�L�Ժɪ��]�֤�x�O���Ҧ�����ާ@�A�T�O�ƾڪ��i�l���ʡC

## �D�n�\��

*   **�w���{�һP���v**
    *   ��� ASP.NET Core Identity ���������ҩM����޲z�C
    *   ��x�n�J�ȭ� `Manager` (�޲z��) �M `Editor` (�s���) ����C
    *   ��Ӫ������v������ (`Manager` �֦��̰��v���A`Editor` �M�`��ӫ~�����޲z)�C
    *   �Ҧ��K�X���~�T���w����ơC

*   **��x����O**
    *   ���ѵn�J�᪽�[�����������A����`�ϥΪ̼ơB�`�ӫ~�ơB�W�[���ӫ~�ơB�`�����ơB�`���ҼƩM����n�J���Ƶ�����έp�ƾڡC

*   **��x�ϥΪ̺޲z (�ȭ� Manager)**
    *   **�ϥΪ̦C��G** ��ܩҦ���x�ϥΪ� (IdentityUser) ���C��A�]�t��ܦW�١B�q�l�l��B����M�b�᪬�A�C
    *   **�s�W�ϥΪ̡G** Manager �i�H���s�[�J����x�����Ыرb���A�]�w��l�K�X�A�ë��� `Manager`�B`Editor` �� `User` ����C
    *   **�s��ϥΪ̡G** Manager �i�H�ק�ϥΪ���ܦW�١B�q�l�l��B�b����w/���ꪬ�A�A�ýվ�䨤��C�P�ɴ��Ѫ������]�K�X�\��C
    *   **�W����ܦW�١G** �ϥΪ̤�����ܪ��W�� (`DisplayName`) �x�s�b�W�ߪ� `UserProfile` ��ƪ��A���v�T Identity �֤ߪ� `UserName` (�P Email �O���@�P�Ω�n�J)�C

*   **�ӤH�b��޲z (Manager �P Editor)**
    *   �w�n�J����x�ϥΪ̥i�H�d�ݦۤv���򥻸�T�C
    *   �w���ק�ۤv���K�X�A�æ����\/���ѰT�����ܡC

*   **�ӫ~�����޲z (Manager ���� CRUD�AEditor �d��)**
    *   ��ӫ~�������i��إߡBŪ���B��s�B�R�� (CRUD) �ާ@�C
    *   �]�t�����W�٩M�y�z�C

*   **�ӫ~���Һ޲z (Manager ���� CRUD�AEditor �d��)**
    *   ��ӫ~�����Ҷi��إߡBŪ���B��s�B�R�� (CRUD) �ާ@�C
    *   �]�t���ҦW�١C

*   **�ӫ~�޲z (Manager �P Editor ���� CRUD)**
    *   **�ӫ~�C��G** ��ܩҦ����޿�R�����ӫ~�A�]�t�W�١B����B�w�s�B�W�[���A�B�D�ϡB���ݤ����M���ҡC�䴩�j�M�B����/���ҿz��M�W�[���A�z��C
    *   **�s�W�ӫ~�G** ���\�s�W�ӫ~�A�]�t���ݩʡB��ܤ����M�h�Ӽ��ҡA�ä䴩�W�Ǧh�i�Ϥ��ó]�w�D�ϡC
    *   **�s��ӫ~�G** �����s��ӫ~��T�A�]�A�޲z�{���Ϥ� (�R���B�]�w�D��)�B�W�Ƿs�Ϥ��A�H�έק�����M���ҡC
    *   **�޿�R���ӫ~�G** �N�ӫ~�аO�� `IsDeleted = true`�A�ӫD���z�R���A�H�O�d�ƾکM�l���ʡC

*   **�]�֤�x**
    *   �ԲӰO���Ҧ����n����x�ާ@ (�ϥΪ̳Ы�/�s��B�ӫ~ CRUD�B���� CRUD�B���� CRUD�B�n�J/�n�X�B�K�X�קﵥ)�C
    *   ���Ѻ��������� `Manager` ����d�ݡB�j�M�B�z�� (�̾ާ@�����B�ؼй���B�ɶ��d��) �M�����s����x�C
    *   �Ҧ��ާ@�����w�������ܡC

*   **�ϥΪ̤����P����**
    *   �ĥ� Bootstrap 5 �c�ءA�����T�����]�p�A�A�����P�]�Ƥؤo�C
    *   �Ҧ���槡��ƫȤ�ݩM���A�������ҡC
    *   �Τ@���ާ@���\�P���ѰT�����ܡC

## �޳N��

*   **�}�o�ج[�G** ASP.NET Core 8 (MVC)
*   **�ƾڮw�G** SQL Server (�ϥ� Entity Framework Core)
*   **�������һP���v�G** ASP.NET Core Identity
*   **�e�� UI�G** Bootstrap 5
*   **��L�w�G**
    *   jQuery & jQuery Validation (�Ω�Ȥ�ݪ������)
    *   Newtonsoft.Json (�Ω�]�֤�x�����ԲӸ�T�ǦC��)

## �}�l�ϥ�

### ���M����

�b���a�B�榹�M�סA�z�ݭn�w�ˡG

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (�� SQL Server LocalDB/Express)
*   (�i��) [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/zh-tw/sql/ssms/download-sql-server-management-studio-ssms) �� [Azure Data Studio](https://docs.microsoft.com/zh-tw/sql/azure-data-studio/) �H�޲z��Ʈw

### �J���x�s�w

```bash
git clone https://github.com/�z��GitHub�ϥΪ̦W��/MerchantBackend.git
cd MerchantBackend

��Ʈw�]�w

�t�m�s���r��G
�}�� appsettings.json �ɮסA�b ConnectionStrings �϶����A�N YOUR_SERVER_NAME �������z�� SQL Server ��ҦW�١C

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MerchantBackendDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "FileUploadSettings": {
    "ProductImageUploadFolder": "images/products" // �ӫ~�Ϥ��x�s���|
  }
}
IGNORE_WHEN_COPYING_START
content_copy
download
Use code with caution.
Json
IGNORE_WHEN_COPYING_END

���ܡG �p�G�ϥ� SQL Server ���ҡA�бN Trusted_Connection=True ������ User ID=YourUser;Password=YourPassword;�C

�����Ʈw�E���G
�b�M�ת��ڥؿ��U (�Y MerchantBackend.csproj �Ҧb��Ƨ�)�A����H�U�R�O�ӳЫةҦ���Ʈw��M���p�G

dotnet ef database update
IGNORE_WHEN_COPYING_START
content_copy
download
Use code with caution.
Bash
IGNORE_WHEN_COPYING_END

�o�|�۰ʰ���Ҧ��ݳB�z���E�� (InitialIdentitySetup, AddAuditLogTable, AddUserProfileTable, AddCategoriesAndTagsTables, AddProductsAndRelations)�C

�ؤl��l�ƾڡG
�M�ױҰʮɡA�|�۰ʹB�� DbInitializer �ӳЫعw�]������ (Manager, Editor, User) �M�@�Ӫ�l�� Manager �b���C

���n�G ��l Manager �b�����q�l�l��O admin@example.com�C

�Эק� SeedData/DbInitializer.cs ���� managerPassword �ܼơA�]�w�@�ӲŦX Identity �������j�K�X�A�ðO���C

// SeedData/DbInitializer.cs �������q
string managerUserEmail = "admin@example.com";
string managerPassword = "YourSecurePassword123!"; // <-- �аȥ��ק惡�K�X�I
IGNORE_WHEN_COPYING_START
content_copy
download
Use code with caution.
C#
IGNORE_WHEN_COPYING_END
�B�����ε{��

�b�M�ת��ڥؿ��U�A����H�U�R�O�G

dotnet run
IGNORE_WHEN_COPYING_START
content_copy
download
Use code with caution.
Bash
IGNORE_WHEN_COPYING_END

���ε{���N�|�b https://localhost:PORT (����ݤf���|��ܦb�׺ݾ���) �ҰʡC

�ϥΫ��n

�X�ݫ�x�G
�b�s�������X�� https://localhost:PORT�C�p�G���n�J�A�z�N�Q�۰ʭ��w�V��n�J�����C

�n�J�G
�ϥαz�b DbInitializer.cs ���]�w�� Manager �b�� (�w�] admin@example.com) �M�K�X�n�J�C

����P�v���G

Manager (�޲z��)�G �֦��Ҧ���x�\�઺�����v�� (�ϥΪ̺޲z�B�ӫ~ CRUD�B���� CRUD�B���� CRUD�B�]�֤�x�d�ݡB�ӤH�b��޲z)�C

Editor (�s���)�G �֦��ӫ~�޲z�B�����d�ݡB���Ҭd�ݡB�ӤH�b��޲z���v���C�L�k�X�ݨϥΪ̺޲z�M�]�֤�x�C

User (�@��ϥΪ�)�G �L�k�n�J����x�t�ΡC

�M�׵��c����

Controllers/: ASP.NET Core MVC ����A�B�z HTTP �ШD�M�~���޿�C

Models/: �]�t�Ҧ� ViewModels (�Ҧp UserViewModel, ProductCreateViewModel ��) �M�@�ǳq�ι��� (AuditLog, UserProfile)�C

Models/Products/: �]�t�ӫ~����������ҫ� (Product, ProductImage, Category, Tag, ProductTag)�C

Data/: �]�t��Ʈw�W�U�� (ApplicationDbContext) �M Entity Framework Core �E���ɮסC

Services/: �]�t���ε{���A�ȡA�Ҧp IAuditService �M AuditService�C

Identity/: �]�t�X�R�� ApplicationUser (�p�G���ӨM�w�X�R IdentityUser)�C

IdentityLocalizations/: �]�t�۩w�q�� IdentityErrorDescriber�A�Ω󤤤�� Identity �����~�T���C

SeedData/: �]�t DbInitializer ���O�A�Ω��Ʈw����l�ƾںؤl�C

Views/: �]�t Razor View �ɮסA�Ω��V�ϥΪ̤����C

wwwroot/: �R�A�ɮצs��B (CSS, JavaScript, �Ϥ��A�Ҧp images/products)�C

appsettings.json: ���ε{���t�m�ɮ� (��Ʈw�s���r��B�ɮפW�Ǹ��|��)�C

���ӥi�઺�X�i (Roadmap)

�ϥΪ��s������ (�q�ӫe�x)�G

��� Web API�G �إ߿W�ߪ� ASP.NET Core Web API �M�סA���Ѹ�Ƶ��e�ݮج[ (React, Angular, Vue.js ��) �c�ت��q�ӫe�x�C

JWT �������ҡG ���e�� API ��{��� JWT ���������ҩM���v�C

�q�l�l�����һP�q���G

���ϥΪ̵��U�B�K�X���]�B�q�l�l���ܧ󵥾ާ@�����q�l�l��o�e�\��C

�Ӳɫ��v���޲z�G

����n�� (Claims) �ε��� (Policies) ��{��ӽo���v������C

�ӫ~�h�ܼ�/SKU �޲z�G

�䴩���P�C��B�ؤo���ӫ~�ܼƪ��w�s�޲z�C

�q��޲z�t�ΡG

�K�[�q��ЫءB�B�z�B�l�ܡB�h�ڵ��\��C

�Ȥ�A�ȼҲաG

�޲z�Ȥ�ԸߡB��D���C

����P�ƾڤ��R�G

���~�ȼƾڥͦ��Ϫ�M���i�C

���p�}���G

�����ε{�����Ѧ۰ʤƳ��p�� IIS�BAzure �� Docker �����x���}���C

�\�i��

���M�צb MIT �\�i�ҤU�o���C�Ա��аѾ\ LICENSE �ɮסC

�Ʊ�o�� README.md ��z�������U�I�z�i�H�N��ƻs��z�� GitHub �M�׮ڥؿ��U�� README.md �ɮפ��C�O�o�N https://github.com/�z��GitHub�ϥΪ̦W��/MerchantBackend.git �������z��ڪ��x�s�w�챵�C