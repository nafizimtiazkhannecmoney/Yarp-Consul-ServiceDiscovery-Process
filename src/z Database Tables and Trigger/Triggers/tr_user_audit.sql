/* Version  : 0.0.1 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Trigger for User Table - Handles audit trail for INSERT and UPDATE operations
******************************************************************************/

-- Drop trigger if exists
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'tr_user_audit')
    DROP TRIGGER tr_user_audit
GO

CREATE TRIGGER tr_user_audit
ON [dbo].[User]
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
        -- This creates the initial audit record with UserVer = 0.
        INSERT INTO [dbo].[UserAudit]
        (
              UserId
            , UserVer
            , IsActive
            , EnvId
            , FsmStateId
            , FsmActionId
            , LegalEntityId
            , ModifiedUserId
            , ModifiedOn
            , LoginName
            , Password
            , FirstName
            , LastName
            , Phone
            , Email
            , IsAllowLogin
            , IsDisabled
            , UserInfo
        )
        SELECT
              i.UserId
            , i.UserVer
            , i.IsActive
            , i.EnvId
            , i.FsmStateId
            , i.FsmActionId
            , i.LegalEntityId
            , i.ModifiedUserId
            , i.ModifiedOn
            , i.LoginName
            , i.Password
            , i.FirstName
            , i.LastName
            , i.Phone
            , i.Email
            , i.IsAllowLogin
            , i.IsDisabled
            , i.UserInfo
        FROM INSERTED AS i;
    END
    
    -- ======================================================================
    -- Logic for UPDATE operations
    -- The DELETED table contains rows for an UPDATE.
    -- ======================================================================
    ELSE
    BEGIN
        -- Step 1: Increment the UserVer in the main [dbo].[User] table.
        -- We join with the INSERTED table to only affect the rows that were just updated.
        UPDATE u
        SET
            UserVer = u.UserVer + 1,
            ModifiedOn = GETDATE()
        FROM [dbo].[User] AS u
        INNER JOIN INSERTED AS i ON u.UserId = i.UserId;
        
        -- Step 2: Insert the updated row into the [dbo].[UserAudit] table.
        -- We join back to the main table to ensure we get the newly incremented UserVer.
        INSERT INTO [dbo].[UserAudit]
        (
              UserId
            , UserVer
            , IsActive
            , EnvId
            , FsmStateId
            , FsmActionId
            , LegalEntityId
            , ModifiedUserId
            , ModifiedOn
            , LoginName
            , Password
            , FirstName
            , LastName
            , Phone
            , Email
            , IsAllowLogin
            , IsDisabled
            , UserInfo
        )
        SELECT
              u.UserId
            , u.UserVer                       -- This will be the incremented version
            , u.IsActive
            , u.EnvId
            , u.FsmStateId
            , u.FsmActionId
            , u.LegalEntityId
            , u.ModifiedUserId
            , u.ModifiedOn                    -- This will be the updated timestamp
            , u.LoginName
            , u.Password
            , u.FirstName
            , u.LastName
            , u.Phone
            , u.Email
            , u.IsAllowLogin
            , u.IsDisabled
            , u.UserInfo
        FROM INSERTED AS i
        INNER JOIN [dbo].[User] AS u ON i.UserId = u.UserId;
    END
END;
GO