/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Generic Map Audit Table - Corrected to match GenericMap table structure
*                 Tracks historical changes to relationship mappings
******************************************************************************/

-- Drop table if exists
DROP TABLE IF EXISTS [dbo].[GenericMapAudit]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create GenericMapAudit table matching GenericMap table structure
CREATE TABLE [dbo].[GenericMapAudit]
(
    -- Primary key fields for audit table (no IDENTITY - stores historical IDs)
      GenericMapId              BIGINT                              NOT NULL                                    -- Matches GenericMap.GenericMapId (BIGINT)
    , GenericMapVer             INT                                 NOT NULL                    DEFAULT 0       -- Version for audit tracking
    
    -- Standard columns (matching GenericMap table exactly)
    , IsActive                  INT                                 NOT NULL                    DEFAULT 1       -- Changed from BIT to INT
    , EnvId                     INT                                 NOT NULL                                    -- Removed default -1
    , ModifiedUserId            INT                                 NOT NULL                                    -- Removed default -1
    , ModifiedOn                DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- Changed from DATETIME to DATETIME2
    , ValidFrom                 DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- Changed from DATETIME to DATETIME2
    , ValidTo                   DATETIME2                           NOT NULL                    DEFAULT DATEADD(year, 1, GETDATE())  -- Changed from DATETIME to DATETIME2
    
    -- Relationship mapping fields (matching GenericMap table exactly)
    , FromTypeId                INT                                 NOT NULL                                    -- Removed default -1
    , FromId                    INT                                 NOT NULL                                    -- Removed default -1
    , FromVersion               INT                                 NOT NULL                                    -- Removed default 0
    , ToTypeId                  INT                                 NOT NULL                                    -- Removed default -1
    , ToId                      INT                                 NOT NULL                                    -- Removed default -1
    , ToVersion                 INT                                 NOT NULL                                    -- Removed default 0
    
    -- Relationship properties (matching GenericMap table exactly)
    , IsPrimary                 INT                                 NOT NULL                                    -- Changed from BIT to INT, removed default
    , SortOrder                 INT                                 NOT NULL                                    -- Removed default 0
    , Description               NVARCHAR(2048)                      NOT NULL                                    -- Changed from VARCHAR to NVARCHAR, removed default
    
    -- Primary key constraint (composite key for audit table)
    CONSTRAINT [pk_generic_map_audit] PRIMARY KEY CLUSTERED 
    (
        [GenericMapId] ASC,
        [GenericMapVer] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
) ON [PRIMARY]
GO