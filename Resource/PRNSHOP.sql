-- Tạo database
CREATE DATABASE ShopBanCa;
GO
USE ShopBanCa;
GO

-- Tài khoản người dùng
CREATE TABLE Account (
    UserID NVARCHAR(20) PRIMARY KEY,                -- VD: U001
    Username NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100),
    Role NVARCHAR(20) CHECK (Role IN ('Admin', 'Manager', 'Staff')) NOT NULL,
    Status BIT NOT NULL DEFAULT 1                   -- Trạng thái hoạt động
);

-- Khách hàng
CREATE TABLE Customer (
    CustomerID NVARCHAR(20) PRIMARY KEY,            -- VD: CUS001
    FullName NVARCHAR(100),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(255),
    Status BIT NOT NULL DEFAULT 1
);

-- Loại sản phẩm
CREATE TABLE Category (
    CategoryID NVARCHAR(20) PRIMARY KEY,            -- VD: CAT001
    CategoryName NVARCHAR(100),
    Description NVARCHAR(255),
    Status BIT NOT NULL DEFAULT 1
);

-- Cá cảnh
CREATE TABLE Fish (
    FishID NVARCHAR(20) PRIMARY KEY,                -- VD: F001
    FishName NVARCHAR(100) NOT NULL,
    Species NVARCHAR(100),
    Color NVARCHAR(50),
    Size NVARCHAR(20),
    Price DECIMAL(10, 2) NOT NULL,
    QuantityAvailable INT NOT NULL DEFAULT 0,
    Description NVARCHAR(255),
    ImageURL NVARCHAR(255),
    CategoryID NVARCHAR(20) FOREIGN KEY REFERENCES Category(CategoryID),
    Status BIT NOT NULL DEFAULT 1
);

-- Phụ kiện hồ cá
CREATE TABLE AquariumAccessory (
    AccessoryID NVARCHAR(20) PRIMARY KEY,           -- VD: AC001
    AccessoryName NVARCHAR(100) NOT NULL,
    CategoryID NVARCHAR(20)FOREIGN KEY REFERENCES Category(CategoryID),
    Description NVARCHAR(255),
    Price DECIMAL(10, 2) NOT NULL,
    QuantityAvailable INT NOT NULL DEFAULT 0,
    ImageURL NVARCHAR(255),
    Status BIT NOT NULL DEFAULT 1
);

-- Đơn hàng
CREATE TABLE [Order] (
    OrderID NVARCHAR(20) PRIMARY KEY,               -- VD: ORD001
    CustomerID NVARCHAR(20) FOREIGN KEY REFERENCES Customer(CustomerID),
    CreatedBy NVARCHAR(20) FOREIGN KEY REFERENCES Account(UserID),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(12,2),
    Status NVARCHAR(50) DEFAULT N'Pending'          -- Giữ nguyên kiểu text mô tả trạng thái đơn hàng
);

-- Chi tiết đơn hàng: Cá cảnh
CREATE TABLE OrderDetail (
    OrderDetailID NVARCHAR(20) PRIMARY KEY,         -- VD: OD001
    OrderID NVARCHAR(20) FOREIGN KEY REFERENCES [Order](OrderID),
    FishID NVARCHAR(20) FOREIGN KEY REFERENCES Fish(FishID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalPrice AS (Quantity * UnitPrice) PERSISTED
);

-- Chi tiết đơn hàng: Phụ kiện
CREATE TABLE OrderAccessoryDetail (
    OrderAccessoryDetailID NVARCHAR(20) PRIMARY KEY,    -- VD: OAD001
    OrderID NVARCHAR(20) FOREIGN KEY REFERENCES [Order](OrderID),
    AccessoryID NVARCHAR(20) FOREIGN KEY REFERENCES AquariumAccessory(AccessoryID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalPrice AS (Quantity * UnitPrice) PERSISTED
);

-- Nhập kho: Cá cảnh
CREATE TABLE InventoryTransaction (
    TransactionID NVARCHAR(20) PRIMARY KEY,         -- VD: TRF001
    FishID NVARCHAR(20) FOREIGN KEY REFERENCES Fish(FishID),
    Quantity INT NOT NULL,
    UnitCost DECIMAL(10,2) NOT NULL,
    TotalCost AS (Quantity * UnitCost) PERSISTED,
    CreatedBy NVARCHAR(20) FOREIGN KEY REFERENCES Account(UserID),
    TransactionDate DATETIME DEFAULT GETDATE()
);

-- Nhập kho: Phụ kiện
CREATE TABLE AccessoryTransaction (
    TransactionID NVARCHAR(20) PRIMARY KEY,         -- VD: TRA001
    AccessoryID NVARCHAR(20) FOREIGN KEY REFERENCES AquariumAccessory(AccessoryID),
    Quantity INT NOT NULL,
    UnitCost DECIMAL(10,2) NOT NULL,
    TotalCost AS (Quantity * UnitCost) PERSISTED,
    CreatedBy NVARCHAR(20) FOREIGN KEY REFERENCES Account(UserID),
    TransactionDate DATETIME DEFAULT GETDATE()
);

-- =========================
-- DATA: Sample Accounts
-- =========================
INSERT INTO Account (UserID, Username, PasswordHash, PhoneNumber, FullName, Role, Status)
VALUES
('U001', 'admin', 'admin123', '0900000000', 'Nguyen Van A', 'Admin', 1),
('U002', 'manager1', 'manager123', '0900000001', 'Dinh Van B', 'Manager', 1),
('U003', 'staff1', 'staff123', '0900000002', 'Tran Thi C', 'Staff', 1),
('U004', 'staff2', 'staff123', '0900000003', 'Ngo The D', 'Staff', 1);

-- =========================
-- DATA: Categories
-- =========================
INSERT INTO Category (CategoryID, CategoryName, Description, Status)
VALUES
('CAT001', 'Freshwater Fish', 'Fish that live in freshwater, easy to raise', 1),
('CAT002', 'Saltwater Fish', 'Colorful saltwater fish, harder to raise', 1),
('CAT003', 'Water Filter Accessories', 'Filters, air pumps... for aquariums', 1),
('CAT004', 'Aquarium Decoration', 'Plastic plants, rocks, wood, LED lights...', 1),
('CAT005', 'Tanks & Devices', 'Glass tanks, heaters, air pumps...', 1);

-- =========================
-- DATA: Fish
-- =========================
INSERT INTO Fish (FishID, FishName, Species, Color, Size, Price, QuantityAvailable, Description, ImageURL, CategoryID, Status)
VALUES
('F001', 'Betta Halfmoon', 'Betta', 'Red', 'Small', 50000, 20, 'Tail shaped like a half moon, suitable for solo tanks', NULL, 'CAT001', 1),
('F002', 'Guppy Full Red', 'Guppy', 'Full Red', 'Small', 25000, 40, 'Friendly fish, easy to raise in groups', NULL, 'CAT001', 1),
('F003', 'Neon King Blue', 'Tetra', 'Blue metallic', 'Small', 20000, 30, 'Small fish, swims beautifully in schools', NULL, 'CAT001', 1),
('F004', 'Ali Lemon Yellow', 'Cichlid', 'Bright Yellow', 'Medium', 60000, 15, 'Energetic, lives in groups', NULL, 'CAT001', 1),
('F005', 'Clown Fish', 'Amphiprion', 'Orange with white stripes', 'Small', 70000, 10, 'Nemo fish, lives in saltwater tanks', NULL, 'CAT002', 1);

-- =========================
-- DATA: Aquarium Accessories
-- =========================
INSERT INTO AquariumAccessory (AccessoryID, AccessoryName, CategoryID, Description, Price, QuantityAvailable, ImageURL, Status)
VALUES
('AC001', 'Mini Water Filter', 'CAT003', 'Suitable for 20–40L tanks, silent operation', 150000, 15, NULL, 1),
('AC002', 'Plastic Aquatic Plant', 'CAT004', 'Non-toxic plastic plant for aquariums', 30000, 25, NULL, 1),
('AC003', 'LED Color-Changing Light', 'CAT004', 'RGB 7-color LED, fits 60–80cm tanks', 120000, 10, NULL, 1),
('AC004', 'Natural Gravel Pack', 'CAT004', 'Small rocks for tank bottom, 1kg pack', 20000, 30, NULL, 1),
('AC005', 'Glass Tank 60x30x30', 'CAT005', '5mm thick glass, suitable for small fish', 400000, 5, NULL, 1),
('AC006', 'Dual Air Pump', 'CAT005', 'Creates oxygen for large aquariums', 100000, 12, NULL, 1);

-- =========================
-- DATA: Customers
-- =========================
INSERT INTO Customer (CustomerID, FullName, Phone, Email, Address, Status)
VALUES
('CUS001', 'Nguyen Van A', '0901234567', 'a@gmail.com', 'Ha Noi', 1),
('CUS002', 'Le Thi B', '0909876543', 'b@gmail.com', 'Ho Chi Minh', 1);

-- =========================
-- DATA: Orders
-- =========================
INSERT INTO [Order] (OrderID, CustomerID, CreatedBy, OrderDate, TotalAmount, Status)
VALUES
('ORD001', 'CUS001', 'U003', '2025-07-01', 180000, 'Completed'),
('ORD002', 'CUS002', 'U004', '2025-07-02', 290000, 'Pending');

-- =========================
-- DATA: OrderDetail (Fish)
-- =========================
INSERT INTO OrderDetail (OrderDetailID, OrderID, FishID, Quantity, UnitPrice)
VALUES
('OD001', 'ORD001', 'F001', 2, 50000),
('OD002', 'ORD001', 'F002', 3, 25000),
('OD003', 'ORD002', 'F003', 5, 20000);

-- =========================
-- DATA: OrderAccessoryDetail
-- =========================
INSERT INTO OrderAccessoryDetail (OrderAccessoryDetailID, OrderID, AccessoryID, Quantity, UnitPrice)
VALUES
('OAD001', 'ORD001', 'AC001', 1, 150000),
('OAD002', 'ORD002', 'AC003', 2, 120000),
('OAD003', 'ORD002', 'AC006', 1, 100000);

-- =========================
-- DATA: Inventory Transactions (Fish)
-- =========================
INSERT INTO InventoryTransaction (TransactionID, FishID, Quantity, UnitCost, CreatedBy, TransactionDate)
VALUES
('TRF001', 'F001', 30, 30000, 'U002', '2025-06-25'),
('TRF002', 'F002', 50, 15000, 'U002', '2025-06-25'),
('TRF003', 'F003', 40, 12000, 'U002', '2025-06-26');

-- =========================
-- DATA: Accessory Transactions
-- =========================
INSERT INTO AccessoryTransaction (TransactionID, AccessoryID, Quantity, UnitCost, CreatedBy, TransactionDate)
VALUES
('TRA001', 'AC001', 20, 100000, 'U002', '2025-06-25'),
('TRA002', 'AC003', 10, 80000, 'U002', '2025-06-26'),
('TRA003', 'AC006', 15, 70000, 'U002', '2025-06-27');