﻿USE [master]
GO
/*******************************************************************************
   Drop database if it exists
********************************************************************************/
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'AssignmentPRN')
BEGIN
	ALTER DATABASE AssignmentPRN SET OFFLINE WITH ROLLBACK IMMEDIATE;
	ALTER DATABASE AssignmentPRN SET ONLINE;
	DROP DATABASE AssignmentPRN;
END

GO

CREATE DATABASE AssignmentPRN
GO

USE AssignmentPRN
GO

/********************************************************************************
	Drop tables if exists
*******************************************************************************/
DECLARE @sql nvarchar(MAX) 
SET @sql = N'' 

SELECT @sql = @sql + N'ALTER TABLE ' + QUOTENAME(KCU1.TABLE_SCHEMA) 
    + N'.' + QUOTENAME(KCU1.TABLE_NAME) 
    + N' DROP CONSTRAINT ' -- + QUOTENAME(rc.CONSTRAINT_SCHEMA)  + N'.'  -- not in MS-SQL
    + QUOTENAME(rc.CONSTRAINT_NAME) + N'; ' + CHAR(13) + CHAR(10) 
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC 

INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU1 
    ON KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG  
    AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA 
    AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME 

EXECUTE(@sql) 

GO
DECLARE @sql2 NVARCHAR(max)=''

SELECT @sql2 += ' Drop table ' + QUOTENAME(TABLE_SCHEMA) + '.'+ QUOTENAME(TABLE_NAME) + '; '
FROM   INFORMATION_SCHEMA.TABLES
WHERE  TABLE_TYPE = 'BASE TABLE'

Exec Sp_executesql @sql2 
GO



-- Tạo bảng nhà cung cấp
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactInfo NVARCHAR(255),
    Email NVARCHAR(100) UNIQUE NULL,
	Avatar NVARCHAR(255) NULL,
    Phone NVARCHAR(15) NULL,
		IsApproved BIT NOT NULL DEFAULT 0,

	IsDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng kho hàng (Kho thuộc quyền quản lý của Supplier)
CREATE TABLE Warehouses (
    WarehouseID INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(255),
    Capacity INT NULL,
    SupplierID INT NOT NULL,
		IsApproved BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
		IsDeleted BIT NOT NULL DEFAULT 0

);

-- Tạo bảng danh mục sản phẩm
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
		IsDeleted BIT NOT NULL DEFAULT 0

);

-- Tạo bảng sản phẩm (Mỗi sản phẩm thuộc về 1 Supplier)
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    CategoryID INT NOT NULL,
    QuantityInStock INT DEFAULT 0,
    Price DECIMAL(18,2) NOT NULL,
	    Avatar NVARCHAR(255) NULL,
    SupplierID INT NOT NULL,
		IsDeleted BIT NOT NULL DEFAULT 0,
	IsApproved BIT NOT NULL DEFAULT 0,
    Description NVARCHAR(255) NULL,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
);

-- Tạo bảng kho hàng sản phẩm (Supplier quản lý sản phẩm trong kho)
CREATE TABLE Inventory (
    InventoryID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    WarehouseID INT NOT NULL,
    SupplierID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    StockStatus NVARCHAR(60) DEFAULT 'Nhập',
    LastUpdated DATETIME DEFAULT GETDATE(),
		IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
);

-- Tạo bảng vai trò người dùng
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(20) UNIQUE NOT NULL
);
-- Tạo bảng người dùng
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15),
    DateOfBirth DATE NULL,
    Gender NVARCHAR(10) NULL,
    RoleID INT NOT NULL,
    Avatar NVARCHAR(255) NULL,
	Address NVARCHAR(255) NULL,
		IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID),
);
CREATE TABLE ApiKeys (
    ApiKeyID INT IDENTITY(1,1) PRIMARY KEY,  -- Mã định danh API Key
    UserID INT NOT NULL,                      -- Liên kết với User
    ApiKey NVARCHAR(255) NOT NULL,            -- Giá trị API Key
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),  -- Thời gian tạo API Key
    IsActive BIT NOT NULL DEFAULT 1,          -- Trạng thái của API Key (kích hoạt hay không)
    FOREIGN KEY (UserID) REFERENCES Users(UserID) -- Khóa ngoại liên kết với bảng Users
);

-- Tạo bảng đơn hàng
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATETIME DEFAULT GETDATE(),
    UserID INT,
    Status NVARCHAR(20) DEFAULT 'Chờ xử lý',
		IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Tạo bảng chi tiết đơn hàng
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    WarehouseID INT NOT NULL,
			IsDeleted BIT NOT NULL DEFAULT 0,
    PriceAtOrder DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID)
);

-- Tạo bảng nhật ký giao dịch
CREATE TABLE TransactionLogs (
    TransactionID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    WarehouseID INT NOT NULL,
    SupplierID INT NOT NULL,
    ChangeType NVARCHAR(10) NOT NULL,
    QuantityChanged INT NOT NULL,
    ChangeDate DATETIME DEFAULT GETDATE(),
    UserID INT,
    Remarks NVARCHAR(255) NULL,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
CREATE TABLE UserSupplier (
    UserID INT PRIMARY KEY,
    SupplierID INT UNIQUE NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID) ON DELETE CASCADE
);

-- ✅ Chèn dữ liệu mẫu vào bảng Roles
INSERT INTO Roles (RoleName) VALUES ('Admin'), ('User'), ('Supplier');






-- ✅ Chèn dữ liệu mẫu vào bảng Suppliers
INSERT INTO Suppliers (SupplierName, ContactInfo, Email, Phone) 
VALUES 
('Công ty ABC', N'123 Đường ABC, TP. HCM', 'abccompany@example.com', '0901234567')



INSERT INTO Users (Username, Password, FullName, Email, Phone, DateOfBirth, Gender, RoleID, Avatar, Address) 
VALUES 
(N'admin', '123', N'Le Xuan Hoang Nam', 'lenam7546@gmail.com', '0987654321', '1990-01-01', 'Male', 1, 'D:\FPTU\Kì5\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Avar\IMG_1071.JPG', N'Hà Nội'),
(N'user', '123', N'Hoang Van Duy', 'lnam22871@gmail.com', '0971234567', '1995-05-10', 'Female', 2, 'D:\FPTU\Kì5\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Avar\IMG_1071.JPG', N'Hồ Chí Minh'),
(N'supplier', '123', N'Thuy Duong', 'user02@example.com', '0962345678', '1998-09-15', 'Male', 3, 'D:\FPTU\Kì5\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Avar\IMG_1071.JPG', N'Đà Nẵng');

-- Sửa lại bảng UserSupplier để chèn dữ liệu đúng
INSERT INTO UserSupplier (UserID, SupplierID) VALUES 
(3, 1)  -- User ID 3 thuộc Supplier ID 1
-- ✅ Chèn dữ liệu mẫu vào bảng Users (Thêm địa chỉ)

INSERT INTO Categories (CategoryName, Description) 
VALUES 
(N'Máy tính & Laptop', N'Laptop, PC, linh kiện máy tính'),
(N'Điện thoại & Tablet', N'Smartphone, tablet, phụ kiện'),
(N'Thời trang Nam', N'Quần áo, giày dép, phụ kiện nam'),
(N'Thời trang Nữ', N'Quần áo, giày dép, phụ kiện nữ'),
(N'Đồ gia dụng', N'Nồi cơm điện, máy xay sinh tố, quạt điện'),
(N'Thực phẩm', N'Rau củ, thịt cá, đồ uống'),
(N'Sách & Văn phòng phẩm', N'Sách giáo khoa, bút, vở, đồ dùng học tập'),
(N'Sức khỏe & Làm đẹp', N'Mỹ phẩm, thực phẩm chức năng, dụng cụ thể thao'),
(N'Mẹ & Bé', N'Sữa bột, tã, quần áo trẻ em'),
(N'Ô tô & Xe máy', N'Xe ô tô, xe máy, phụ kiện xe');


-- ✅ Chèn dữ liệu mẫu vào bảng Products
INSERT INTO Products (ProductName, CategoryID, QuantityInStock, Price, SupplierID, Description) 
VALUES 
(N'Laptop Dell XPS', 1, 50, 15, 1, N'Laptop Dell XPS 13, màn hình 13 inch, core i7'),
(N'Laptop Acer Nito', 1, 50, 15, 1, N'Laptop Dell XPS 13, màn hình 13 inch, core i7'),
(N'Máy giặt Samsung', 3, 30, 500, 1, N'Máy giặt Samsung 8kg, cửa trước'),
(N'Máy giặt LG', 3, 30, 500, 1, N'Máy giặt Samsung 8kg, cửa trước');



-- ✅ Chèn dữ liệu mẫu vào bảng Warehouses (Kho hàng phải có trước)
INSERT INTO Warehouses (WarehouseName, Location, Capacity, SupplierID) 
VALUES 
(N'Kho Hà Nội', N'123 Nguyễn Trãi, Hà Nội', 1000, 1),
(N'Kho TP.HCM', N'456 Lý Thường Kiệt, TP.HCM', 1500, 1);

-- Chèn hàng vào kho (Supplier quản lý)
INSERT INTO Inventory (ProductID, WarehouseID, SupplierID, Quantity) 
VALUES 
(1, 1, 1, 10),  -- Laptop Dell XPS trong kho Hà Nội, Supplier ABC
(2, 2, 1, 20);  -- Máy giặt Samsung trong kho TP.HCM, Supplier XYZ

-- Chèn đơn hàng
INSERT INTO Orders (UserID, Status) VALUES (2, N'Chờ xử lý');
-- Chèn chi tiết đơn hàng
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, WarehouseID, PriceAtOrder) 
VALUES 
(1, 1, 1, 1, 15000000);
