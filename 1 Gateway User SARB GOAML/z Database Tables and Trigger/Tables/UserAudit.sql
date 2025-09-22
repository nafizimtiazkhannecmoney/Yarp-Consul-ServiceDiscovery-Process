/* Version  : 0.0.1 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: User Audit Table - Corrected to match User table structure
******************************************************************************/

-- Drop table if exists
DROP TABLE IF EXISTS [dbo].[UserAudit]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create UserAudit table matching User table structure
CREATE TABLE [dbo].[UserAudit]
(
    -- Primary key fields for audit table
      UserId                    BIGINT                              NOT NULL                                -- Matches User.UserId (BIGINT)
    , UserVer                   INT                                 NOT NULL                    DEFAULT 0   -- Version for audit tracking
    
    -- Standard columns (matching User table exactly)
    , IsActive                  INT                                 NOT NULL                    DEFAULT 1   -- Changed from BIT to INT
    , EnvId                     INT                                 NOT NULL                    DEFAULT -1  -- Missing field added
    , FsmStateId                INT                                 NOT NULL                    DEFAULT -1  -- Missing field added  
    , FsmActionId               INT                                 NOT NULL                    DEFAULT -1  -- Missing field added
    , LegalEntityId             INT                                 NOT NULL                    DEFAULT -1
    , ModifiedUserId            INT                                 NOT NULL                    DEFAULT -1
    , ModifiedOn                DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- Changed from DATETIME to DATETIME2
    
    -- Text fields - matching User table data types and sizes exactly
    , LoginName                 NVARCHAR(255)                       NOT NULL                    DEFAULT ''  -- Changed from VARCHAR(128) to NVARCHAR(255)
    , Password                  NVARCHAR(512)                       NOT NULL                    DEFAULT ''  -- Changed from VARCHAR(512) to NVARCHAR(512)
    , FirstName                 NVARCHAR(255)                       NOT NULL                    DEFAULT ''  -- Changed from VARCHAR(128) to NVARCHAR(255)
    , LastName                  NVARCHAR(255)                       NOT NULL                    DEFAULT ''  -- Changed from VARCHAR(128) to NVARCHAR(255)
    , Phone                     NVARCHAR(50)                        NOT NULL                    DEFAULT ''  -- Changed from VARCHAR(64) to NVARCHAR(50)
    , Email                     NVARCHAR(320)                       NOT NULL                    DEFAULT ''  -- Changed from VARCHAR(128) to NVARCHAR(320)
    
    -- Boolean-like fields (matching User table INT type)
    , IsAllowLogin              INT                                 NOT NULL                    DEFAULT 1   -- Changed from BIT to INT
    , IsDisabled                INT                                 NOT NULL                    DEFAULT 0   -- Changed from BIT to INT
    
    -- JSON field (renamed to match User table)
    , UserInfo                  NVARCHAR(MAX)                       NOT NULL                    DEFAULT '{}'-- Renamed from OtherInformation
    
    -- Primary key constraint (composite key for audit table)
    CONSTRAINT [pk_user_audit] PRIMARY KEY CLUSTERED 
    (
        [UserId] ASC,
        [UserVer] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
) ON [PRIMARY]
GO