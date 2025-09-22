/* Version  : 0.0.1 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-09
* Description	: User Table
******************************************************************************/

-- Drop table if exists
DROP TABLE IF EXISTS [dbo].[User]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create User table with all PostgreSQL columns mapped
CREATE TABLE [dbo].[User]
(
    -- Primary key - matches PostgreSQL identity starting at 100000
      UserId                    BIGINT                              IDENTITY(100000, 1)         NOT NULL
    
    -- Standard columns
    , UserVer                   INT                                 NOT NULL                    DEFAULT 0
    , IsActive                  INT                                 NOT NULL                    DEFAULT 1
    , EnvId                     INT                                 NOT NULL                    DEFAULT -1      -- id_env_key
    , FsmStateId                INT                                 NOT NULL                    DEFAULT -1      -- id_fsm_state_key  
    , FsmActionId               INT                                 NOT NULL                    DEFAULT -1      -- id_fsm_action_key
    , LegalEntityId             INT                                 NOT NULL                    DEFAULT -1      -- id_legal_entity_key
    , ModifiedUserId            INT                                 NOT NULL                    DEFAULT -1      -- id_user_mod_key
    , ModifiedOn                DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- dtt_mod
    
    -- Text fields - using reasonable varchar limits for performance
    , LoginName                 NVARCHAR(255)                       NOT NULL                    DEFAULT ''      -- tx_login_name
    , Password                  NVARCHAR(512)                       NOT NULL                    DEFAULT ''      -- tx_password  
    , FirstName                 NVARCHAR(255)                       NOT NULL                    DEFAULT ''      -- tx_first_name
    , LastName                  NVARCHAR(255)                       NOT NULL                    DEFAULT ''      -- tx_last_name
    , Phone                     NVARCHAR(50)                        NOT NULL                    DEFAULT ''      -- tx_phone
    , Email                     NVARCHAR(320)                       NOT NULL                    DEFAULT ''      -- tx_email (max email length)
    
    -- Boolean-like fields (keeping as INT to match PostgreSQL int4)
    , IsAllowLogin              INT                                 NOT NULL                    DEFAULT 1       -- is_allow_login
    , IsDisabled                INT                                 NOT NULL                    DEFAULT 0       -- is_disabled
    
    -- JSON field
    , UserInfo                  NVARCHAR(MAX)                       NOT NULL                    DEFAULT '{}'    -- j_user_info
    
    -- Primary key constraint
    CONSTRAINT [pk_user] PRIMARY KEY CLUSTERED 
    (
        [UserId] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
) ON [PRIMARY]
GO

