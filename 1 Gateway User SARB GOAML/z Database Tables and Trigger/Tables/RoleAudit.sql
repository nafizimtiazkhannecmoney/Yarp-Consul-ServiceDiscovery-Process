/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Role Audit Table - Corrected to match Role table structure
*                 Tracks historical changes to roles
******************************************************************************/

-- Drop table if exists
DROP TABLE IF EXISTS [dbo].[RoleAudit]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create RoleAudit table matching Role table structure
CREATE TABLE [dbo].[RoleAudit]
(
    -- Primary key fields for audit table (no IDENTITY - stores historical IDs)
      RoleId                    BIGINT                              NOT NULL                                    -- Matches Role.RoleId (BIGINT)
    , RoleVer                   INT                                 NOT NULL                    DEFAULT 0       -- Version for audit tracking
    
    -- Standard columns (matching Role table exactly)
    , IsActive                  INT                                 NOT NULL                    DEFAULT 1       -- Changed from BIT to INT
    , EnvId                     INT                                 NOT NULL                                    -- Removed default -1
    , ModifiedUserId            INT                                 NOT NULL                                    -- Removed default -1
    , ModifiedOn                DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- Changed from DATETIME to DATETIME2
    
    -- Text fields (matching Role table exactly)
    , RoleName                  NVARCHAR(128)                       NOT NULL                                    -- Changed from VARCHAR to NVARCHAR, removed default
    , Description               NVARCHAR(1024)                      NOT NULL                                    -- Changed from VARCHAR to NVARCHAR, removed default
    
    -- Primary key constraint (composite key for audit table)
    CONSTRAINT [pk_role_audit] PRIMARY KEY CLUSTERED 
    (
        [RoleId] ASC,
        [RoleVer] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
) ON [PRIMARY]
GO