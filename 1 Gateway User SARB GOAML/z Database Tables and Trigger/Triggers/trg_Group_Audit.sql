/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Trigger for Group Table - Corrected to match table structure
*                 Handles audit trail for INSERT and UPDATE operations
******************************************************************************/

-- Drop trigger if exists
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_Group_Audit')
    DROP TRIGGER trg_Group_Audit
GO

CREATE TRIGGER trg_Group_Audit
ON [dbo].[Group]
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
        -- This creates the initial audit record with GroupVer = 0.
        INSERT INTO [dbo].[GroupAudit]
        (
              GroupId
            , GroupVer
            , IsActive
            , EnvId
            , ModifiedUserId
            , ModifiedOn
            , GroupTypeId
            , GroupName
            , Description
        )
        SELECT
              i.GroupId
            , i.GroupVer
            , i.IsActive
            , i.EnvId
            , i.ModifiedUserId
            , i.ModifiedOn
            , i.GroupTypeId
            , i.GroupName
            , i.Description
        FROM INSERTED AS i;
    END
    
    -- ======================================================================
    -- Logic for UPDATE operations
    -- The DELETED table contains rows for an UPDATE.
    -- ======================================================================
    ELSE
    BEGIN
        -- Step 1: Increment the GroupVer in the main [dbo].[Group] table.
        -- We join with the INSERTED table to only affect the rows that were just updated.
        UPDATE g
        SET
            GroupVer = g.GroupVer + 1,
            ModifiedOn = GETDATE()
        FROM [dbo].[Group] AS g
        INNER JOIN INSERTED AS i ON g.GroupId = i.GroupId;
        
        -- Step 2: Insert the updated row into the [dbo].[GroupAudit] table.
        -- We join back to the main table to ensure we get the newly incremented GroupVer.
        INSERT INTO [dbo].[GroupAudit]
        (
              GroupId
            , GroupVer
            , IsActive
            , EnvId
            , ModifiedUserId
            , ModifiedOn
            , GroupTypeId
            , GroupName
            , Description
        )
        SELECT
              g.GroupId
            , g.GroupVer                      -- This will be the incremented version
            , g.IsActive
            , g.EnvId
            , g.ModifiedUserId
            , g.ModifiedOn                    -- This will be the updated timestamp
            , g.GroupTypeId
            , g.GroupName
            , g.Description
        FROM INSERTED AS i
        INNER JOIN [dbo].[Group] AS g ON i.GroupId = g.GroupId;
    END
END;
GO