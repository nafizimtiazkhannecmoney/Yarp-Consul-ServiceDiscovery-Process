-- Drop procedure if exists
IF OBJECT_ID('dbo.get_all_roles', 'P') IS NOT NULL
    DROP PROCEDURE dbo.get_all_roles;
GO

CREATE PROCEDURE dbo.get_all_roles
    @rs_out NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all roles excluding system roles
    SELECT @rs_out = (
        SELECT 
            R.RoleId AS [id],
            R.RoleId AS roleId,
            R.RoleName AS roleName
        FROM [dbo].[Role] R
        WHERE LOWER(R.RoleName) != LOWER('System')
          AND LOWER(R.RoleName) != LOWER('SA')
          AND LOWER(R.RoleName) != LOWER('TECH_ADMIN')
        FOR JSON PATH
    );
END;
GO