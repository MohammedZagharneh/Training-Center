CREATE PROCEDURE SP_GetUsersList
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.UserID,
        U.Username,
        U.Role,
        U.IsActive,
        U.PersonID,
        P.FirstName,
        P.LastName,
        P.NationalNumber,
        P.Email,
        P.PhoneNumber
    FROM Users U
    INNER JOIN People P ON P.PersonID = U.PersonID
    ORDER BY U.UserID;
END;
GO


--The transaction of creating a person record before making him a user is in business service
CREATE PROCEDURE SP_Register
@username NVARCHAR(100),
@passwordHash NVARCHAR(200),
@role NVARCHAR(50),
@isActive BIT,
@personID INT
AS

BEGIN
SET NOCOUNT ON;

INSERT INTO Users(Username, PasswordHash, Role, IsActive, PersonID)
VALUES(@username, @passwordHash, @role, @isActive, @personID);

SELECT SCOPE_IDENTITY() AS UserID;
END


GO

--For Login
CREATE PROCEDURE SP_GetUserDetails
@Username NVARCHAR(100)
AS
BEGIN

SET NOCOUNT ON;

SELECT UserID, Username, PasswordHash, Role, IsActive, PersonID FROM Users WHERE Username = @Username;

END


GO 