/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Trigger for Role Table - Corrected to match table structure
*                 Handles audit trail for INSERT and UPDATE operations
******************************************************************************/

-- Drop trigger if exists
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_Role_Audit')
    DROP TRIGGER trg_Role_Audit
GO

CREATE TRIGGER trg_Role_Audit
ON [dbo].[Role]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- ======================================================================
    -- Logic for INSERT operations
    -- The DELETED table is empty for an INSERT.
    -- ======================================================================
    IF NOT EXISTS (SELECT 1 FROM DELETED)
    BEGIN
        -- Insert the new row directly from the INSERTED table.
        -- This creates the initial audit record with RoleVer = 0.
        INSERT INTO [dbo].[RoleAudit]
        (
              RoleId
            , RoleVer
            , IsActive
            , EnvId
            , ModifiedUserId
            , ModifiedOn
            , RoleName
            , Description
        )
        SELECT
              i.RoleId
            , i.RoleVer
            , i.IsActive
            , i.EnvId
            , i.ModifiedUserId
            , i.ModifiedOn
            , i.RoleName
            , i.Description
        FROM INSERTED AS i;
    END
    
    -- ======================================================================
    -- Logic for UPDATE operations
    -- The DELETED table contains rows for an UPDATE.
    -- ======================================================================
    ELSE
    BEGIN
        -- Step 1: Increment the RoleVer in the main [dbo].[Role] table.
        -- We join with the INSERTED table to only affect the rows that were just updated.
        UPDATE r
        SET
            RoleVer = r.RoleVer + 1,
            ModifiedOn = GETDATE()
        FROM [dbo].[Role] AS r
        INNER JOIN INSERTED AS i ON r.RoleId = i.RoleId;
        
        -- Step 2: Insert the updated row into the [dbo].[RoleAudit] table.
        -- We join back to the main table to ensure we get the newly incremented RoleVer.
        INSERT INTO [dbo].[RoleAudit]
        (
              RoleId
            , RoleVer
            , IsActive
            , EnvId
            , ModifiedUserId
            , ModifiedOn
            , RoleName
            , Description
        )
        SELECT
              r.RoleId
            , r.RoleVer                       -- This will be the incremented version
            , r.IsActive
            , r.EnvId
            , r.ModifiedUserId
            , r.ModifiedOn                    -- This will be the updated timestamp
            , r.RoleName
            , r.Description
        FROM INSERTED AS i
        INNER JOIN [dbo].[Role] AS r ON i.RoleId = r.RoleId;
    END
END;
GO