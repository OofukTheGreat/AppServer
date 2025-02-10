Use master                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               Use master
Go
IF EXISTS (SELECT * FROM sys.databases WHERE name = N'AppServer_DB')
BEGIN
    DROP DATABASE AppServer_DB;
END
Go
Create Database AppServer_DB
Go
Use AppServer_DB
Go

--טבלת שחקנים--
CREATE TABLE Players (
    PlayerId INT IDENTITY(1,1) PRIMARY KEY, --מפתח ראשי--
    Email NVARCHAR(100), --אימייל--
    [Password] NVARCHAR(100), --סיסמה--
    DisplayName NVARCHAR(20), --שם מוצג--
    ProfilePicture nvarchar(100), --תמונת פרופיל--
    IsAdmin BIT --האם מנהל--
);

--טבלת סטטוסים--
CREATE TABLE Statuses (
    StatusId INT IDENTITY(1,1) PRIMARY KEY, --מפתח ראשי--
    StatusName NVARCHAR(100) --שם סטטוס--
);

--טבלת דרגות קושי--
CREATE TABLE Difficulties (
    DifficultyId INT IDENTITY(1,1) PRIMARY KEY, --מפתח ראשי--
    DifficultyName NVARCHAR(100) --שם דרגת קושי--
);

--טבלת שלבים--
CREATE TABLE Levels (
    LevelId INT IDENTITY(1,1) PRIMARY KEY, --מפתח ראשי--
    Title NVARCHAR(100), --שם שלב--
    Layout NVARCHAR(2000), --מבנה שלב--
    CreatorId INT, --קוד שחקן יוצר--
    StatusId INT, --קוד ססטוס--
    DifficultyId INT, --קוד דרגת קושי--
    FOREIGN KEY (CreatorId) REFERENCES Players(PlayerId), --קישור לטבלת שחקנים--
    FOREIGN KEY (StatusId) REFERENCES Statuses(StatusId), --קישור לטבלת סטטוסים--
    FOREIGN KEY (DifficultyId) REFERENCES Difficulties(DifficultyId), --קישור לטבלת דרגות קושי--
);

--טבלת תוצאות--
CREATE TABLE Scores (
    PlayerId INT, --קוד שחקן--
    LevelId INT, --קוד שלב--
    FOREIGN KEY (PlayerId) REFERENCES Players(PlayerId), --קישור לטבלת שחקנים--
    FOREIGN KEY (LevelId) REFERENCES Levels(LevelId), --קישור לטבלת שלבים--
    CONSTRAINT PK_Scores PRIMARY KEY (PlayerId,LevelId), --קישור מפתחות זרים--
    [Time] NVARCHAR(20) --זמן לסיום שלב--
);

-- Create a login for the admin user
CREATE LOGIN [AdminLogin] WITH PASSWORD = '1234';
Go

-- Create a user in the TamiDB database for the login
CREATE USER [AdminUser] FOR LOGIN [AdminLogin];
Go

-- Add the user to the db_owner role to grant admin privileges
ALTER ROLE db_owner ADD MEMBER [AdminUser];
Go

INSERT INTO Players (Email,[Password],DisplayName,IsAdmin) VALUES ('ofekrom1@gmail.com','1234','Admin',1)
INSERT INTO Levels (Title, Layout, CreatorId, StatusId, DifficultyId) VALUES ('Cherry', '6,4,.5,2,2,1,.4,1,1,2,2,.3,2,2,1,2,.2,1,4,1,2,.1,3,2,3,1,.0,1,2,3,2,2,.0,1,1,4,1,3,.0,4,1,5,.1,2,3,3,1,.', 1, 1, 10)
INSERT INTO Levels (Title, Layout, CreatorId, StatusId, DifficultyId) VALUES ('Heart', '1,1,1,1,1,.0,5,.0,5,.1,3,1,.2,1,2,.', 1, 1, 10)

--EF Code
/*
scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=AppServer_DB;User ID=AdminLogin;Password=1234;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context DBContext -DataAnnotations -force
*/

SELECT * FROM Players