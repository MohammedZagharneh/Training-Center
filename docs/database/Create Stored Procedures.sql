CREATE PROCEDURE SP_RegisterNewPersonAsUser
@NationalNumber NVARCHAR(50),
@FirstName NVARCHAR(50),
@LastName NVARCHAR(50),
@Email NVARCHAR(100),
@BirthDate DATE,
@PhoneNumber NVARCHAR(50),
@Username NVARCHAR(100),
@PasswordHash NVARCHAR(100)
AS

BEGIN 
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

BEGIN TRY

   BEGIN TRANSACTION;

INSERT INTO People(NationalNumber, FirstName,LastName, Email, BirthDate, PhoneNumber)
VALUES(@NationalNumber, @FirstName, @LastName, @Email, @BirthDate, @PhoneNumber);

DECLARE @PersonID INT = SCOPE_IDENTITY();

INSERT INTO Users(Username, Password, PersonID)
VALUES(@Username, @PasswordHash, @PersonID);

DECLARE @UserID INT = SCOPE_IDENTITY();

   COMMIT TRANSACTION;

END TRY

BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
END CATCH

END;

GO


CREATE PROCEDURE SP_RegisterExistingPersonAsUser
@Username NVARCHAR(100),
@PasswordHash NVARCHAR(100),
@PersonID INT
AS

BEGIN
SET NOCOUNT ON;
SET XACT_ABORT ON;

INSERT INTO Users(Username, Password, PersonID)
VALUES(@Username, @PasswordHash, @PersonID);

SELECT SCOPE_IDENTITY() AS UserID;
END


GO

CREATE PROCEDURE SP_Login
@Username NVARCHAR(100)
AS
BEGIN

SELECT UserID, Username, Password, PersonID FROM Users WHERE Username = @Username;

END


GO 

CREATE PROCEDURE SP_UpdatePassword
@UserID INT,
@NewPasswordHash NVARCHAR(100)
AS 
BEGIN

SET NOCOUNT ON;

UPDATE Users SET Password = @NewPasswordHash WHERE UserID = @UserID;

SELECT @@ROWCOUNT AS RowsAffected;

END