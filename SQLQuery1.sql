-- Create Database
IF EXISTS(SELECT * from sys.databases WHERE name='StreamingServiceDB')
BEGIN
    DROP DATABASE StreamingServiceDB;
END
CREATE DATABASE StreamingServiceDB;
GO

USE StreamingServiceDB;
GO

-- User Table for Registration
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(50) NOT NULL
);
GO

-- Movie Table (Metadata stored in DynamoDB, but SQL table can be used for relational information)
CREATE TABLE Movies (
    MovieID INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Genre VARCHAR(50) NOT NULL,
    Director VARCHAR(100),
    ReleaseDate DATE,
    S3Path VARCHAR(255) NOT NULL,  -- S3 Path for the movie file
    UploadedBy INT FOREIGN KEY REFERENCES Users(UserID),
    UploadDate DATETIME DEFAULT GETDATE()
);
GO

-- Comments Table
CREATE TABLE Comments (
    CommentID INT IDENTITY(1,1) PRIMARY KEY,
    MovieID INT FOREIGN KEY REFERENCES Movies(MovieID) ON DELETE CASCADE,
    UserID INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
    CommentText VARCHAR(500) NOT NULL,
    CommentDate DATETIME DEFAULT GETDATE(),
    LastModified DATETIME
);
GO

-- Rating Table
CREATE TABLE Ratings (
    RatingID INT IDENTITY(1,1) PRIMARY KEY,
    MovieID INT FOREIGN KEY REFERENCES Movies(MovieID) ON DELETE CASCADE,
    UserID INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
    RatingValue INT CHECK (RatingValue BETWEEN 1 AND 10) NOT NULL,  -- Ratings 1-10
    RatingDate DATETIME DEFAULT GETDATE()
);
GO



-- Scaffold-DbContext "Data Source=movie-app.c5ueo6sqo8m2.us-east-1.rds.amazonaws.com,1433;Database=movieapp;TrustServerCertificate=true;User ID=admin;Password=KlEmvY4EuPoaq5DDZ22l;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models