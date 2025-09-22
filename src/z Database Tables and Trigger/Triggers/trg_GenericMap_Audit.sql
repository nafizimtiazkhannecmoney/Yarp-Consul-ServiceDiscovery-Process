/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Trigger for GenericMap Table - Corrected to match table structure
*                 Handles audit trail for INSERT and UPDATE operations
******************************************************************************/

-- Drop trigger if exists
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_GenericMap_Audit')
    DROP TRIGGER trg_GenericMap_Audit
GO

CREATE TRIGGER trg_GenericMap_Audit
ON [dbo].[GenericMap]
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
        -- This creates the initial audit record with GenericMapVer = 0.
        INSERT INTO [dbo].[GenericMapAudit]
        (
              GenericMapId
            , GenericMapVer
            , IsActive
            , EnvId
            , ModifiedUserId
            , ModifiedOn
            , ValidFrom
            , ValidTo
            , FromTypeId
            , FromId
            , FromVersion
            , ToTypeId
            , ToId
            , ToVersion
            , IsPrimary
            , SortOrder
            , Description
        )
        SELECT
              i.GenericMapId
            , i.GenericMapVer
            , i.IsActive
            , i.EnvId
            , i.ModifiedUserId
            , i.ModifiedOn
            , i.ValidFrom
            , i.ValidTo
            , i.FromTypeId
            , i.FromId
            , i.FromVersion
            , i.ToTypeId
            , i.ToId
            , i.ToVersion
            , i.IsPrimary
            , i.SortOrder
            , i.Description
        FROM INSERTED AS i;
    END
    
    -- ======================================================================
    -- Logic for UPDATE operations
    -- The DELETED table contains rows for an UPDATE.
    -- ======================================================================
    ELSE
    BEGIN
        -- Step 1: Increment the GenericMapVer in the main [dbo].[GenericMap] table.
        -- We join with the INSERTED table to only affect the rows that were just updated.
        UPDATE gm
        SET
            GenericMapVer = gm.GenericMapVer + 1,
            ModifiedOn = GETDATE()
        FROM [dbo].[GenericMap] AS gm
        INNER JOIN INSERTED AS i ON gm.GenericMapId = i.GenericMapId;
        
        -- Step 2: Insert the updated row into the [dbo].[GenericMapAudit] table.
        -- We join back to the main table to ensure we get the newly incremented GenericMapVer.
        INSERT INTO [dbo].[GenericMapAudit]
        (
              GenericMapId
            , GenericMapVer
            , IsActive
            , EnvId
            , ModifiedUserId
            , ModifiedOn
            , ValidFrom
            , ValidTo
            , FromTypeId
            , FromId
            , FromVersion
            , ToTypeId
            , ToId
            , ToVersion
            , IsPrimary
            , SortOrder
            , Description
        )
        SELECT
              gm.GenericMapId
            , gm.GenericMapVer                -- This will be the incremented version
            , gm.IsActive
            , gm.EnvId
            , gm.ModifiedUserId
            , gm.ModifiedOn                   -- This will be the updated timestamp
            , gm.ValidFrom
            , gm.ValidTo
            , gm.FromTypeId
            , gm.FromId
            , gm.FromVersion
            , gm.ToTypeId
            , gm.ToId
            , gm.ToVersion
            , gm.IsPrimary
            , gm.SortOrder
            , gm.Description
        FROM INSERTED AS i
        INNER JOIN [dbo].[GenericMap] AS gm ON i.GenericMapId = gm.GenericMapId;
    END
END;
GO