/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Role Table - Corrected to match PostgreSQL t_role structure
******************************************************************************/

-- Drop table if exists
DROP TABLE IF EXISTS [dbo].[Role]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create Role table matching PostgreSQL t_role structure
CREATE TABLE [dbo].[Role]
(
    -- Primary key - matches PostgreSQL int8 identity starting at 100000
      RoleId                    BIGINT                              IDENTITY(100000, 1)         NOT NULL        -- id_role_key
    
    -- Standard columns
    , RoleVer                   INT                                 NOT NULL                    DEFAULT 0       -- id_role_ver
    , IsActive                  INT                                 NOT NULL                    DEFAULT 1       -- is_active
    , EnvId                     INT                                 NOT NULL                                    -- id_env_key
    , ModifiedUserId            INT                                 NOT NULL                                    -- id_user_mod_key
    , ModifiedOn                DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- dtt_mod
    
    -- Text fields
    , RoleName                  NVARCHAR(128)                       NOT NULL                                    -- tx_role_name
    , Description               NVARCHAR(1024)                      NOT NULL                                    -- tx_desc
    
    -- Primary key constraint
    CONSTRAINT [pk_role] PRIMARY KEY CLUSTERED 
    (
        [RoleId] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
) ON [PRIMARY]
GO