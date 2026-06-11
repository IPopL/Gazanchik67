-- Модуль 1. База данных ООО «Обувь»
-- Скрипт можно выполнить в SSMS, если не используешь автосоздание при запуске приложения

IF DB_ID('ShoesStore') IS NULL
    CREATE DATABASE ShoesStore;
GO

USE ShoesStore;
GO

IF OBJECT_ID('dbo.Roles', 'U') IS NULL
CREATE TABLE dbo.Roles (
    RoleID INT NOT NULL PRIMARY KEY,
    Role NVARCHAR(50) NOT NULL
);

IF OBJECT_ID('dbo.Users', 'U') IS NULL
CREATE TABLE dbo.Users (
    Role INT NOT NULL,
    Name NVARCHAR(80) NOT NULL PRIMARY KEY,
    Login NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (Role) REFERENCES dbo.Roles(RoleID)
);

IF OBJECT_ID('dbo.Categories', 'U') IS NULL
CREATE TABLE dbo.Categories (
    ID INT NOT NULL PRIMARY KEY,
    Category NVARCHAR(50) NOT NULL
);

IF OBJECT_ID('dbo.Manufacturers', 'U') IS NULL
CREATE TABLE dbo.Manufacturers (
    ID INT NOT NULL PRIMARY KEY,
    Manufacturer NVARCHAR(50) NOT NULL
);

IF OBJECT_ID('dbo.Suppliers', 'U') IS NULL
CREATE TABLE dbo.Suppliers (
    ID INT NOT NULL PRIMARY KEY,
    Supplier NVARCHAR(50) NOT NULL
);

IF OBJECT_ID('dbo.Tovar', 'U') IS NULL
CREATE TABLE dbo.Tovar (
    Article NVARCHAR(6) NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    QuantityMeasure NVARCHAR(50) NOT NULL,
    Price INT NOT NULL,
    SupplierID INT NOT NULL,
    ManufacturerID INT NOT NULL,
    Category INT NOT NULL,
    Discount INT NOT NULL,
    Amount INT NOT NULL,
    Description NVARCHAR(100) NOT NULL,
    Photo NVARCHAR(50) NULL,
    CONSTRAINT FK_Tovar_Suppliers FOREIGN KEY (SupplierID) REFERENCES dbo.Suppliers(ID),
    CONSTRAINT FK_Tovar_Manufacturers FOREIGN KEY (ManufacturerID) REFERENCES dbo.Manufacturers(ID),
    CONSTRAINT FK_Tovar_Categories FOREIGN KEY (Category) REFERENCES dbo.Categories(ID)
);

IF OBJECT_ID('dbo.DeliveryAddress', 'U') IS NULL
CREATE TABLE dbo.DeliveryAddress (
    AddressID INT NOT NULL PRIMARY KEY,
    Address NVARCHAR(100) NULL
);

IF OBJECT_ID('dbo.[Order]', 'U') IS NULL
CREATE TABLE dbo.[Order] (
    OrderID INT NOT NULL PRIMARY KEY,
    OrderDate DATETIME2 NOT NULL,
    DeliveryDate DATETIME2 NOT NULL,
    AddressID INT NOT NULL,
    ClientName NVARCHAR(80) NOT NULL,
    Code INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Order_DeliveryAddress FOREIGN KEY (AddressID) REFERENCES dbo.DeliveryAddress(AddressID),
    CONSTRAINT FK_Order_Users FOREIGN KEY (ClientName) REFERENCES dbo.Users(Name)
);

IF OBJECT_ID('dbo.OrderDetails', 'U') IS NULL
CREATE TABLE dbo.OrderDetails (
    OrderID INT NOT NULL,
    Article NVARCHAR(6) NOT NULL,
    Amount INT NOT NULL,
    CONSTRAINT PK_OrderDetails PRIMARY KEY (OrderID, Article),
    CONSTRAINT FK_OrderDetails_Order FOREIGN KEY (OrderID) REFERENCES dbo.[Order](OrderID),
    CONSTRAINT FK_OrderDetails_Tovar FOREIGN KEY (Article) REFERENCES dbo.Tovar(Article)
);
GO
