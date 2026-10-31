IF DB_ID('TrainingCenter') IS NULL
BEGIN 
CREATE DATABASE TrainingCenter
END

GO 

USE TrainingCenter;

GO

CREATE TABLE People
(
PersonID INT IDENTITY(1,1) NOT NULL,
NationalNumber NVARCHAR(50) NOT NULL, -- this was added here , not in the ERD or RS
FirstName NVARCHAR(50) NOT NULL,
LastName NVARCHAR(50) NOT NULL,
Email NVARCHAR(100) NULL,
BirthDate DATE NULL,
PhoneNumber NVARCHAR(50) NULL,
RegisteredAt DATE NOT NULL CONSTRAINT DF_RegisteredAt DEFAULT(GETDATE()),

CONSTRAINT PK_People PRIMARY KEY (PersonID),
CONSTRAINT UQ_Email UNIQUE (Email),
CONSTRAINT UQ_NationalNumber UNIQUE(NationalNumber),
CONSTRAINT UQ_PhoneNumber UNIQUE (PhoneNumber)
);

GO

CREATE TABLE Users
(
UserID INT IDENTITY(1,1) NOT NULL,
Username NVARCHAR(100) NOT NULL,
Password NVARCHAR(100) NOT NULL,
PersonID INT NOT NULL,

CONSTRAINT PK_Users PRIMARY KEY (UserID),
CONSTRAINT UQ_Username UNIQUE (Username),
CONSTRAINT FK_Users_People FOREIGN KEY (PersonID) REFERENCES People(PersonID),
CONSTRAINT UQ_Users_PersonID UNIQUE (PersonID)
);

CREATE TABLE Students
(

StudentID INT IDENTITY(1,1) NOT NULL,
StudentStatus NVARCHAR(50) NOT NULL,
PersonID INT NOT NULL,

CONSTRAINT PK_Students PRIMARY KEY (StudentID),
CONSTRAINT CK_StudentStatus CHECK (StudentStatus IN ('Active', 'Suspended', 'Graduated')),
CONSTRAINT FK_Students_People FOREIGN KEY (PersonID) REFERENCES People(PersonID),
CONSTRAINT UQ_Students_PersonID UNIQUE (PersonID)

);


CREATE TABLE Instructors
(
InstructorID INT IDENTITY(1,1) NOT NULL,
Salary DECIMAL(10,2) NOT NULL,
IsActive BIT NOT NULL CONSTRAINT DF_IsActive DEFAULT(1),
PersonID INT NOT NULL,
ManagerID INT NULL,

CONSTRAINT PK_Instructors PRIMARY KEY (InstructorID),
CONSTRAINT CK_Salary CHECK (Salary >= 0),
CONSTRAINT FK_Instructors_People FOREIGN KEY (PersonID) REFERENCES People(PersonID),
CONSTRAINT FK_Instructors_Managers FOREIGN KEY (ManagerID) REFERENCES Instructors(InstructorID),

CONSTRAINT UQ_Instructors_PersonID UNIQUE (PersonID),
CONSTRAINT CK_NoSelfManagement CHECK(ManagerID IS NULL OR ManagerID <> InstructorID)
);


GO 

CREATE TABLE Courses
(
CourseID INT IDENTITY(1,1) NOT NULL,
Title NVARCHAR(100) NOT NULL,
Code NVARCHAR(100) NOT NULL,
Price DECIMAL(10,2) NOT NULL,
Description NVARCHAR(500) NULL,
InstructorID INT NOT NULL

CONSTRAINT PK_Courses PRIMARY KEY (CourseID),
CONSTRAINT UQ_Code UNIQUE (Code),
CONSTRAINT CK_Price CHECK(Price >= 0),
CONSTRAINT FK_Courses_Instructors FOREIGN KEY(InstructorID) REFERENCES Instructors(InstructorID)
);

GO

CREATE TABLE Enrollments
(
EnrollmentID INT IDENTITY(1,1) NOT NULL,
StudentID INT NOT NULL,
CourseID INT NOT NULL,
EnrollmentDate DATE NOT NULL CONSTRAINT DF_EnrollmentDate DEFAULT(GETDATE()),
CompletionDate DATE NULL,
EnrollmentStatus NVARCHAR(50) NOT NULL,

CONSTRAINT PK_Enrollments PRIMARY KEY (EnrollmentID),

CONSTRAINT CK_CompletionDate CHECK(CompletionDate > EnrollmentDate),
CONSTRAINT CK_EnrollmentStatus CHECK(EnrollmentStatus IN ('Active', 'Completed', 'Dropped')),
CONSTRAINT UQ_StudentID_CourseID UNIQUE (StudentID, CourseID),

CONSTRAINT FK_Enrollments_Students FOREIGN KEY(StudentID) REFERENCES Students(StudentID),
CONSTRAINT FK_Enrollments_Courses FOREIGN KEY(CourseID) REFERENCES Courses(CourseID)
);

GO


CREATE INDEX IX_Instructors_ManagerID
ON Instructors(ManagerID);
GO

CREATE INDEX IX_Courses_InstructorID
ON Courses(InstructorID);
GO

--no need for this index because the composite index is enough, it uses the unique index for the left most column which is StudentID
--CREATE INDEX IX_Enrollments_StudentID
--ON Enrollments(StudentID);
--GO

CREATE INDEX IX_Enrollments_CourseID
ON Enrollments(CourseID);
GO