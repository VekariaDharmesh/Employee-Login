-- create database for employee app
CREATE DATABASE EmployeeDB;
GO

USE EmployeeDB;
GO

-- table to store registered employees
CREATE TABLE Employees (
    EmployeeId    INT IDENTITY(1,1) PRIMARY KEY,
    FullName      NVARCHAR(100) NOT NULL,
    EmployeeCode  NVARCHAR(50)  NOT NULL UNIQUE,
    Email         NVARCHAR(100) NOT NULL UNIQUE,
    Password      NVARCHAR(100) NOT NULL,
    CreatedDate   DATETIME NOT NULL DEFAULT GETDATE()
);
GO
