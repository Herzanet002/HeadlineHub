CREATE TABLE dbo.Users
(
    user_id INT PRIMARY KEY IDENTITY(1,1),
    user_name NVARCHAR(20) NOT NULL UNIQUE,
    registration_date DATETIME DEFAULT GETDATE()
);