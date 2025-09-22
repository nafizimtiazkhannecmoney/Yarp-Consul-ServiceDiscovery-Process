/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Group Audit Table - Corrected to match Group table structure
*                 Tracks historical changes to groups
******************************************************************************/

-- Drop table if exists
DROP TABLE IF EXISTS [dbo].[GroupAudit]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create GroupAudit table matching Group table structure
CREATE TABLE [dbo].[GroupAudit]
(
    -- Primary key fields for audit table (no IDENTITY - stores historical IDs)
      GroupId                   BIGINT                              NOT NULL                                    -- Matches Group.GroupId (BIGINT)
    , GroupVer                  INT                                 NOT NULL                    DEFAULT 0       -- Version for audit tracking
    
    -- Standard columns (matching Group table exactly)
    , IsActive                  INT                                 NOT NULL                                    -- Changed from BIT to INT, removed default
    , EnvId                     INT                                 NOT NULL                                    -- Removed default -1
    , ModifiedUserId            INT                                 NOT NULL                                    -- Removed default -1
    , ModifiedOn                DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- Changed from DATETIME to DATETIME2
    , GroupTypeId               INT                                 NOT NULL                                    -- Removed default -1
    
    -- Text fields (matching Group table exactly)
    , GroupName                 NVARCHAR(128)                       NOT NULL                                    -- Changed from VARCHAR to NVARCHAR, removed default
    , Description               NVARCHAR(1024)                      NOT NULL                                    -- Changed from VARCHAR to NVARCHAR, removed default
    
    -- Primary key constraint (composite key for audit table)
    CONSTRAINT [pk_group_audit] PRIMARY KEY CLUSTERED 
    (
        [GroupId] ASC,
        [GroupVer] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
) ON [PRIMARY]
GO