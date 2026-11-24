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

---------------------------------------------------------

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

---------------------------------------------------------

--For Login
CREATE PROCEDURE SP_GetUserDetails
@Username NVARCHAR(100)
AS
BEGIN

SET NOCOUNT ON;

SELECT UserID, Username, PasswordHash, Role, IsActive, PersonID FROM Users WHERE Username = @Username;

END


GO 

---------------------------------------------------------

CREATE PROCEDURE SP_GetRefreshTokenDetails
    @refreshTokenHash NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        RT.RefreshTokenID,
        RT.UserID,
        U.Username,
        RT.ExpiresAt,
        RT.RevokedAt,
        U.IsActive,
        U.Role
    FROM RefreshTokens RT
    INNER JOIN Users U ON RT.UserID = U.UserID
    WHERE RT.RefreshTokenHash = @refreshTokenHash;
END
GO 

---------------------------------------------------------

CREATE PROCEDURE SP_RevokeRefreshToken
    @refreshTokenHash NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET RevokedAt = SYSUTCDATETIME()
    WHERE RefreshTokenHash = @refreshTokenHash
      AND RevokedAt IS NULL;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

--AffectedRows = 1 means it was revoked. 0 means it doesn't exist or was already revoked.

---------------------------------------------------------

CREATE PROCEDURE SP_AddRefreshToken
    @refreshTokenHash NVARCHAR(200),
    @expiresAt DATETIME2,
    @userID INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO RefreshTokens
        (RefreshTokenHash, ExpiresAt, UserID)
    VALUES
        (@refreshTokenHash, @expiresAt, @userID);

    SELECT SCOPE_IDENTITY() AS RefreshTokenID;
END
GO


---------------------------------------------------------

CREATE PROCEDURE SP_ChangePassword
    @userID INT,
    @newPasswordHash NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET PasswordHash = @newPasswordHash
    WHERE UserID = @userID;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

---------------------------------------------------------


CREATE PROCEDURE SP_LogoutFromAllDevices
    @userID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET RevokedAt = SYSUTCDATETIME()
    WHERE UserID = @userID
      AND RevokedAt IS NULL;

    SELECT @@ROWCOUNT AS AffectedRows;
END
