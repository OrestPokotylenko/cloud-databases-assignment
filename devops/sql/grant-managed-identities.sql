SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @apiIdentityName SYSNAME = N'$(ApiIdentityName)';
DECLARE @apiIdentityClientId UNIQUEIDENTIFIER = '$(ApiIdentityClientId)';
DECLARE @apiIdentitySid VARBINARY(16) = CONVERT(VARBINARY(16), @apiIdentityClientId);

IF EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = @apiIdentityName
      AND sid <> @apiIdentitySid
)
BEGIN
    THROW 50001, 'The API database user exists with a different Microsoft Entra client ID.', 1;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = @apiIdentityName
)
BEGIN
    DECLARE @createApiUserSql NVARCHAR(MAX) =
        N'CREATE USER ' + QUOTENAME(@apiIdentityName) +
        N' WITH SID = ' + CONVERT(VARCHAR(MAX), @apiIdentitySid, 1) +
        N', TYPE = E;';

    EXEC sys.sp_executesql @createApiUserSql;
END;

DECLARE @grantApiPermissionsSql NVARCHAR(MAX) =
    N'GRANT SELECT, INSERT, UPDATE, DELETE TO ' + QUOTENAME(@apiIdentityName) + N';';

EXEC sys.sp_executesql @grantApiPermissionsSql;

DECLARE @functionIdentityName SYSNAME = N'$(FunctionIdentityName)';
DECLARE @functionIdentityClientId UNIQUEIDENTIFIER = '$(FunctionIdentityClientId)';
DECLARE @functionIdentitySid VARBINARY(16) = CONVERT(VARBINARY(16), @functionIdentityClientId);

IF EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = @functionIdentityName
      AND sid <> @functionIdentitySid
)
BEGIN
    THROW 50002, 'The Functions database user exists with a different Microsoft Entra client ID.', 1;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = @functionIdentityName
)
BEGIN
    DECLARE @createFunctionUserSql NVARCHAR(MAX) =
        N'CREATE USER ' + QUOTENAME(@functionIdentityName) +
        N' WITH SID = ' + CONVERT(VARCHAR(MAX), @functionIdentitySid, 1) +
        N', TYPE = E;';

    EXEC sys.sp_executesql @createFunctionUserSql;
END;

DECLARE @grantFunctionPermissionsSql NVARCHAR(MAX) =
    N'GRANT SELECT, INSERT, UPDATE TO ' + QUOTENAME(@functionIdentityName) + N';';

EXEC sys.sp_executesql @grantFunctionPermissionsSql;

COMMIT TRANSACTION;
