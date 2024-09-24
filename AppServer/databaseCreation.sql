﻿Use master
Go
IF EXISTS (SELECT * FROM sys.databases WHERE name = N'AppServer')
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
    PlayerId INT PRIMARY KEY,      --מפתח ראשי--
    Email NVARCHAR(100),           --אימייל--
    [Password] NVARCHAR(100),      --סיסמה--
    DisplayName NVARCHAR(100),     --שם מוצג--
    ProfilePicture VARBINARY(MAX), --תמונת פרופיל--
    IsAdmin BOOL                   --האם מנהל--
);

--טבלת שלבים--
CREATE TABLE Levels (
    LevelId INT PRIMARY KEY,
    Title NVARCHAR(100),
    FOREIGN KEY (CreatorId) REFERENCES Players(PlayerId)

-- יצירת טבלת תלמידים
CREATE TABLE Students (
    StudentId INT PRIMARY KEY,        -- מפתח ראשי
    FullName NVARCHAR(100),           -- שם מלא של התלמיד
    DateOfBirth DATE                  -- תאריך לידה של התלמיד
);

-- יצירת טבלת מורים
CREATE TABLE Teachers (
    TeacherId INT PRIMARY KEY,        -- מפתח ראשי
    TeacherName NVARCHAR(100)         -- שם מלא של המורה
);

-- יצירת טבלת מקצועות לימוד
CREATE TABLE Subjects (
    SubjectId INT PRIMARY KEY,        -- מפתח ראשי
    SubjectName NVARCHAR(100),        -- שם המקצוע
    TeacherId INT,                    -- מפתח זר לטבלת מורים
    FOREIGN KEY (TeacherId) REFERENCES Teachers(TeacherId)   -- קישור לטבלת המורים
);

-- יצירת טבלת חדרי לימוד
CREATE TABLE Classrooms (
    ClassroomId INT PRIMARY KEY,      -- מפתח ראשי
    ClassroomName NVARCHAR(100),      -- שם חדר הלימוד
    Capacity INT                      -- כמות מקומות בחדר
);

-- יצירת טבלת רישום
CREATE TABLE Enrollments (
    EnrollmentId INT PRIMARY KEY,     -- מפתח ראשי
    StudentId INT,                    -- מפתח זר לטבלת תלמידים
    SubjectId INT,                    -- מפתח זר לטבלת מקצועות לימוד
    EnrollmentDate DATE,              -- תאריך הרישום
    Grade DECIMAL(5, 2),              -- ציון
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId),   -- קישור לטבלת התלמידים
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId)    -- קישור לטבלת המקצועות
);