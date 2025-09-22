/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Group Table - Corrected to match PostgreSQL t_group structure
******************************************************************************/

-- Drop table if exists
DROP TABLE IF EXISTS [dbo].[Group]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create Group table matching PostgreSQL t_group structure
CREATE TABLE [dbo].[Group]
(
    -- Primary key - matches PostgreSQL int8 identity starting at 100000
      GroupId                   BIGINT                              IDENTITY(100000, 1)         NOT NULL        -- id_group_key
    
    -- Standard columns
    , GroupVer                  INT                                 NOT NULL                    DEFAULT 0       -- id_group_ver
    , IsActive                  INT                                 NOT NULL                                    -- is_active
    , EnvId                     INT                                 NOT NULL                                    -- id_env_key
    , ModifiedUserId            INT                                 NOT NULL                                    -- id_user_mod_key
    , ModifiedOn                DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- dtt_mod
    , GroupTypeId               INT                                 NOT NULL                                    -- id_group_type_value_key
    
    -- Text fields
    , GroupName                 NVARCHAR(128)                       NOT NULL                                    -- tx_group_name
    , Description               NVARCHAR(1024)                      NOT NULL                                    -- tx_desc
    
    -- Primary key constraint
    CONSTRAINT [pk_group] PRIMARY KEY CLUSTERED 
    (
        [GroupId] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
) ON [PRIMARY]
GO

-- Create unique index on GroupName to match PostgreSQL idx_group_name
CREATE UNIQUE NONCLUSTERED INDEX [idx_group_name] 
ON [dbo].[Group] ([GroupName] ASC)
WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    SORT_IN_TEMPDB = OFF, 
    IGNORE_DUP_KEY = OFF, 
    DROP_EXISTING = OFF, 
    ONLINE = OFF, 
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]
GO