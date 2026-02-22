-- 1. Accounts
CREATE TABLE Accounts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(250) UNIQUE, -- Tên đăng nhập
    PassWord NVARCHAR(250), -- Mật khẩu (nên hash trước khi lưu)
    Creation DATETIME, -- Ngày tạo tài khoản
    RefreshToken NVARCHAR(250), -- Token refresh cho auth
    TokenExpired DATETIME, -- Thời gian hết hạn token
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);

-- 2. Customers
CREATE TABLE Customers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT, -- ID auth liên kết
    DateBirth DATETIME, -- Ngày sinh
    Sex NVARCHAR(20), -- Giới tính (Nam/Nữ/Khác)
    Phone NVARCHAR(250) UNIQUE, -- Số điện thoại
    Address NVARCHAR(250), -- Địa chỉ
    Email NVARCHAR(250) UNIQUE, -- Email người dùng
    IDCard NVARCHAR(250) UNIQUE, -- CCCD/CMND
    ReferralCode NVARCHAR(15), -- Mã giới thiệu
    AccumulatedPoints INT, -- Điểm tích lũy từ giới thiệu
    RatingPoints INT, -- Điểm thứ hạng (từ mua hàng/dịch vụ)
    RankMember NVARCHAR(200), -- Thứ hạng thành viên (Vang, Bac, KimCuong,...)
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE Customers ADD CONSTRAINT FK_Customers_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE;

-- 3. Staffs
CREATE TABLE Staffs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT, -- ID auth liên kết
    TypeStaff NVARCHAR(50), -- Loại staff (NhanVien, BacSi, Admin) - thay thế phần TypePerson cho staff
    DateBirth DATETIME, -- Ngày sinh
    Sex NVARCHAR(20), -- Giới tính (Nam/Nữ/Khác)
    Phone NVARCHAR(250), -- Số điện thoại
    Address NVARCHAR(250), -- Địa chỉ
    IDCard NVARCHAR(250), -- CCCD/CMND
    SalesPoints INT, -- Điểm bán hàng (cho nhân viên)
    StaffImage NVARCHAR(MAX), -- Hình ảnh bác sĩ/nhân viên (thêm để tải ảnh lên)
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE Staffs ADD CONSTRAINT FK_Staffs_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE;

-- 4. ServiceTypes
CREATE TABLE ServiceTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ServiceTypeName NVARCHAR(250), -- Tên loại (Massage, ChamSocDa,...)
    ServiceCategory NVARCHAR(20), -- Phân loại (SanPham hoặc DichVu)
    Description NVARCHAR(MAX), -- Mô tả loại (thêm để quản lý dịch vụ tốt hơn)
    DeleteStatus BIT -- Trạng thái xóa mềm (đổi thành BIT)
);

-- 5. Functions
CREATE TABLE Functions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FunctionCode NVARCHAR(250) UNIQUE, -- Mã hàm (ví dụ: FUNC_BOOKING)
    FunctionName NVARCHAR(250), -- Tên chức năng (Quản lý lịch hẹn)
    Description NVARCHAR(MAX), -- Mô tả chức năng (thêm để rõ ràng)
    DeleteStatus BIT -- Trạng thái xóa mềm
);

-- 6. Permissions
CREATE TABLE Permissions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT, -- ID auth (quyền gắn với account đăng nhập)
    FunctionId INT, -- ID chức năng
    IsView BIT, -- Quyền xem
    IsInsert BIT, -- Quyền thêm
    IsUpdate BIT, -- Quyền sửa
    IsDelete BIT, -- Quyền xóa
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE Permissions ADD CONSTRAINT UQ_Permissions_Account_Function UNIQUE (AccountId, FunctionId);
ALTER TABLE Permissions ADD CONSTRAINT FK_Permissions_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE;
ALTER TABLE Permissions ADD CONSTRAINT FK_Permissions_Functions FOREIGN KEY (FunctionId) REFERENCES Functions(Id) ON DELETE CASCADE;

-- 7. Services
CREATE TABLE Services (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ServiceTypeId INT, -- ID loại dịch vụ
    ServiceName NVARCHAR(200), -- Tên dịch vụ
    Description NVARCHAR(MAX), -- Mô tả
    ServiceImage NVARCHAR(MAX), -- Hình ảnh
    Price DECIMAL(18,2), -- Giá dịch vụ
    Duration INT, -- Thời lượng (phút) - thêm để quản lý lịch hẹn tránh trùng
    DeleteStatus BIT -- Trạng thái xóa mềm
);
ALTER TABLE Services ADD CONSTRAINT FK_Services_ServiceTypes FOREIGN KEY (ServiceTypeId) REFERENCES ServiceTypes(Id) ON DELETE CASCADE;

-- 8. Suppliers
CREATE TABLE Suppliers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SupplierName NVARCHAR(200), -- Tên nhà cung cấp
    Address NVARCHAR(250), -- Địa chỉ
    Phone NVARCHAR(250), -- SĐT
    DeleteStatus BIT -- Trạng thái xóa mềm
);

-- 9. Products
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ServiceTypeId INT, -- ID loại sản phẩm
    SupplierId INT, -- ID nhà cung cấp
    ProductName NVARCHAR(250), -- Tên sản phẩm
    Description NVARCHAR(MAX), -- Mô tả
    SellingPrice DECIMAL(18,2), -- Giá bán
    Quantity INT, -- Số lượng tồn kho
    Unit NVARCHAR(50), -- Đơn vị (chai, hộp,...)
    MinimumStock INT, -- Số lượng tối thiểu để cảnh báo hết hàng
    ProductImages NVARCHAR(MAX), -- Hình ảnh
    CostPrice DECIMAL(18,2), -- Giá vốn để tính lợi nhuận
    DeleteStatus BIT -- Trạng thái xóa mềm
);
ALTER TABLE Products ADD CONSTRAINT FK_Products_ServiceTypes FOREIGN KEY (ServiceTypeId) REFERENCES ServiceTypes(Id) ON DELETE CASCADE;
ALTER TABLE Products ADD CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id) ON DELETE SET NULL;

-- 10. Comments
CREATE TABLE Comments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NULL, -- ID sản phẩm (nếu áp dụng)
    ServiceId INT NULL, -- ID dịch vụ (nếu áp dụng)
    CustomerId INT, -- ID khách hàng (comment từ khách)
    CommentContent NVARCHAR(250), -- Nội dung
    Rating INT CHECK (Rating BETWEEN 1 AND 5), -- Đánh giá sao (1-5)
    CreationDate DATETIME, -- Ngày tạo
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE Comments ADD CONSTRAINT FK_Comments_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE SET NULL;
ALTER TABLE Comments ADD CONSTRAINT FK_Comments_Services FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE SET NULL;
ALTER TABLE Comments ADD CONSTRAINT FK_Comments_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE;

-- 11. Vouchers
CREATE TABLE Vouchers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) UNIQUE, -- Mã voucher
    Description NVARCHAR(MAX), -- Mô tả
    VoucherImage NVARCHAR(MAX), -- Hình ảnh
    DiscountValue DECIMAL(5,2), -- Phần trăm giảm
    StartDate DATETIME, -- Bắt đầu
    EndDate DATETIME, -- Kết thúc
    MinimumOrderValue DECIMAL(18,2), -- Giá trị đơn tối thiểu
    MaxValue DECIMAL(18,2), -- Giá trị giảm tối đa
    RankMember NVARCHAR(200), -- Thứ hạng yêu cầu
    RatingPoints INT, -- Điểm mua hàng để đổi
    AccumulatedPoints INT, -- Điểm giới thiệu để đổi
    UsageLimit INT, -- Số lần sử dụng tối đa
    IsActive BIT, -- Trạng thái (0: Inactive, 1: Active)
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);

-- 12. Appointments
CREATE TABLE Appointments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT, -- ID khách hàng
    StaffId INT, -- ID bác sĩ/nhân viên (chọn có lịch trống)
    ServiceId INT, -- ID dịch vụ
    StartTime DATETIME, -- Thời gian bắt đầu
    EndTime DATETIME, -- Thời gian kết thúc
    CreationDate DATETIME, -- Ngày tạo
    Status NVARCHAR(50), -- Trạng thái (DaDat, DangThucHien, Huy, DoiLich)
    PaymentStatus BIT, -- Trạng thái thanh toán (0: Chua, 1: Da)
    DeleteStatus BIT, -- Xóa mềm
    CHECK (EndTime > StartTime)
);
ALTER TABLE Appointments ADD CONSTRAINT FK_Appointments_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE;
ALTER TABLE Appointments ADD CONSTRAINT FK_Appointments_Staffs FOREIGN KEY (StaffId) REFERENCES Staffs(Id) ON DELETE SET NULL;
ALTER TABLE Appointments ADD CONSTRAINT FK_Appointments_Services FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE SET NULL;

-- 13. Carts
CREATE TABLE Carts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT, -- ID khách
    CreationDate DATETIME, -- Ngày tạo
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE Carts ADD CONSTRAINT FK_Carts_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE;

-- 14. CartProducts
CREATE TABLE CartProducts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CartId INT,
    ProductId INT NULL,
    ServiceId INT NULL,
    Quantity INT,
    PriceAtAdd DECIMAL(18,2), -- Giá tại thời điểm thêm
    CreateDate DATETIME, -- Ngày thêm
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE CartProducts ADD CONSTRAINT FK_CartProducts_Carts FOREIGN KEY (CartId) REFERENCES Carts(Id) ON DELETE CASCADE;
ALTER TABLE CartProducts ADD CONSTRAINT FK_CartProducts_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE SET NULL;
ALTER TABLE CartProducts ADD CONSTRAINT FK_CartProducts_Services FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE SET NULL;

-- 15. Wallets
CREATE TABLE Wallets (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT, -- ID khách
    VoucherId INT, -- ID voucher trong ví
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE Wallets ADD CONSTRAINT FK_Wallets_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE;
ALTER TABLE Wallets ADD CONSTRAINT FK_Wallets_Vouchers FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id) ON DELETE CASCADE;

-- 16. AccountSessions
CREATE TABLE AccountSessions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT, -- ID auth
    Token NVARCHAR(MAX),
    DeviceName NVARCHAR(250),
    IP NVARCHAR(250),
    CreateTime DATETIME,
    LastAccess DATETIME, -- Lần truy cập cuối
    DeleteStatus BIT
);
ALTER TABLE AccountSessions ADD CONSTRAINT FK_AccountSessions_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE;

-- 17. Invoices
CREATE TABLE Invoices (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT, -- ID khách
    StaffId INT, -- ID nhân viên/bác sĩ
    VoucherId INT,
    DiscountValue DECIMAL(5,2),
    TotalMoney DECIMAL(18,2),
    DateCreated DATETIME,
    Status NVARCHAR(50), -- Trạng thái thanh toán
    DeleteStatus BIT,
    Type NVARCHAR(50), -- Loại (NhapHang, BanHang)
    OrderStatus NVARCHAR(50), -- Trạng thái đơn
    PaymentMethod NVARCHAR(50), -- Phương thức
    OutstandingBalance DECIMAL(18,2) -- Số dư công nợ
);
ALTER TABLE Invoices ADD CONSTRAINT FK_Invoices_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE SET NULL;
ALTER TABLE Invoices ADD CONSTRAINT FK_Invoices_Staffs FOREIGN KEY (StaffId) REFERENCES Staffs(Id) ON DELETE SET NULL;
ALTER TABLE Invoices ADD CONSTRAINT FK_Invoices_Vouchers FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id) ON DELETE SET NULL;

-- 18. InvoiceDetails
CREATE TABLE InvoiceDetails (
    Id INT PRIMARY KEY IDENTITY(1,1),
    InvoiceId INT,
    ProductId INT NULL,
    ServiceId INT NULL,
    VoucherId INT NULL,
    DiscountValue DECIMAL(5,2),
    Price DECIMAL(18,2), -- Giá chung
    Quantity INT, -- Số lượng
    TotalMoney DECIMAL(18,2),
    Status NVARCHAR(50),
    DeleteStatus BIT,
    Type NVARCHAR(50), -- Loại (Nhap, Ban)
    StatusComment BIT -- Trạng thái comment
);
ALTER TABLE InvoiceDetails ADD CONSTRAINT FK_InvoiceDetails_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE CASCADE;
ALTER TABLE InvoiceDetails ADD CONSTRAINT FK_InvoiceDetails_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE SET NULL;
ALTER TABLE InvoiceDetails ADD CONSTRAINT FK_InvoiceDetails_Services FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE SET NULL;
ALTER TABLE InvoiceDetails ADD CONSTRAINT FK_InvoiceDetails_Vouchers FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id) ON DELETE SET NULL;

-- 19. Clinics
CREATE TABLE Clinics (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ClinicName NVARCHAR(255),
    ClinicStatus BIT, -- Trạng thái (0: Dong, 1: Mo)
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);

-- 20. ClinicStaff
CREATE TABLE ClinicStaff (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ClinicId INT,
    StaffId INT, -- ID nhân viên/bác sĩ
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE ClinicStaff ADD CONSTRAINT UQ_ClinicStaff_Clinic_Staff UNIQUE (ClinicId, StaffId);
ALTER TABLE ClinicStaff ADD CONSTRAINT FK_ClinicStaff_Clinics FOREIGN KEY (ClinicId) REFERENCES Clinics(Id) ON DELETE CASCADE;
ALTER TABLE ClinicStaff ADD CONSTRAINT FK_ClinicStaff_Staffs FOREIGN KEY (StaffId) REFERENCES Staffs(Id) ON DELETE CASCADE;

-- 21. AppointmentAssignments
CREATE TABLE AppointmentAssignments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AppointmentId INT, -- ID lịch hẹn
    ClinicId INT,
    ServiceTypeId INT,
    StaffId INT, -- ID bác sĩ/nhân viên
    ServiceId INT,
    NumberOrder INT, -- Thứ tự
    AssignedDate DATETIME,
    Status BIT,
    DeleteStatus BIT,
    QuantityServices INT, -- Số lượng dịch vụ
    Price DECIMAL(18,2),
    PaymentStatus BIT,
    EquipmentId INT NULL -- Thêm cho tránh trùng thiết bị
);
ALTER TABLE AppointmentAssignments ADD CONSTRAINT FK_AppointmentAssignments_Appointments FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id) ON DELETE CASCADE;
ALTER TABLE AppointmentAssignments ADD CONSTRAINT FK_AppointmentAssignments_Clinics FOREIGN KEY (ClinicId) REFERENCES Clinics(Id) ON DELETE SET NULL;
ALTER TABLE AppointmentAssignments ADD CONSTRAINT FK_AppointmentAssignments_ServiceTypes FOREIGN KEY (ServiceTypeId) REFERENCES ServiceTypes(Id) ON DELETE SET NULL;
ALTER TABLE AppointmentAssignments ADD CONSTRAINT FK_AppointmentAssignments_Staffs FOREIGN KEY (StaffId) REFERENCES Staffs(Id) ON DELETE SET NULL;
ALTER TABLE AppointmentAssignments ADD CONSTRAINT FK_AppointmentAssignments_Services FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE SET NULL;
ALTER TABLE AppointmentAssignments ADD CONSTRAINT FK_AppointmentAssignments_Equipments FOREIGN KEY (EquipmentId) REFERENCES Equipments(Id) ON DELETE SET NULL;

-- 24. StaffShifts
CREATE TABLE StaffShifts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StaffId INT, -- ID nhân viên/bác sĩ
    StartTime DATETIME,
    EndTime DATETIME,
    Date DATE, -- Ngày ca
    Status BIT, -- 0: DaPhan, 1: HoanThanh
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE StaffShifts ADD CONSTRAINT FK_StaffShifts_Staffs FOREIGN KEY (StaffId) REFERENCES Staffs(Id) ON DELETE CASCADE;

-- 25. PerformanceLogs
CREATE TABLE PerformanceLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StaffId INT, -- ID nhân viên
    InvoiceId INT, -- Liên kết hóa đơn
    Commission DECIMAL(18,2), -- Hoa hồng
    Bonus DECIMAL(18,2), -- Thưởng
    LogDate DATETIME,
    Description NVARCHAR(MAX), -- Mô tả
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE PerformanceLogs ADD CONSTRAINT FK_PerformanceLogs_Staffs FOREIGN KEY (StaffId) REFERENCES Staffs(Id) ON DELETE CASCADE;
ALTER TABLE PerformanceLogs ADD CONSTRAINT FK_PerformanceLogs_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE SET NULL;

-- 29. Equipments
CREATE TABLE Equipments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EquipmentName NVARCHAR(250),
    ClinicId INT,
    Status NVARCHAR(20), -- 'SanSang', 'DangSuDung', 'HuHong'
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE Equipments ADD CONSTRAINT FK_Equipments_Clinics FOREIGN KEY (ClinicId) REFERENCES Clinics(Id) ON DELETE CASCADE;

-- 31. ServiceProducts
CREATE TABLE ServiceProducts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ServiceId INT,
    ProductId INT,
	StaffId INT,
    QuantityUsed INT, -- Số lượng dùng per dịch vụ
    DeleteStatus BIT -- Trạng thái xóa mềm (0: Active, 1: Deleted)
);
ALTER TABLE ServiceProducts ADD CONSTRAINT FK_ServiceProducts_Services FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE CASCADE;
ALTER TABLE ServiceProducts ADD CONSTRAINT FK_ServiceProducts_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
ALTER TABLE ServiceProducts ADD CONSTRAINT FK_ServiceProducts_Staffs FOREIGN KEY (StaffId) REFERENCES Staffs(Id) ON DELETE SET NULL;