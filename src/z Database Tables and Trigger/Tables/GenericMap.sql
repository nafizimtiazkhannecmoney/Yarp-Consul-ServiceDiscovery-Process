/* Version  : 0.0.2 */
/******************************************************************************
* Author		: Nafiz Imtiaz Khan
* Date			: 2025-08-25
* Description	: Generic Map Table - Corrected to match PostgreSQL t_generic_map structure
*                 Handles many-to-many relationships between any entities
******************************************************************************/

-- Drop table if exists
DROP TABLE IF EXISTS [dbo].[GenericMap]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create GenericMap table matching PostgreSQL t_generic_map structure
CREATE TABLE [dbo].[GenericMap]
(
    -- Primary key - matches PostgreSQL int8 identity starting at 100000
      GenericMapId              BIGINT                              IDENTITY(100000, 1)         NOT NULL        -- id_generic_map_key
    
    -- Standard columns
    , GenericMapVer             INT                                 NOT NULL                    DEFAULT 0       -- id_generic_map_ver
    , IsActive                  INT                                 NOT NULL                    DEFAULT 1       -- is_active
    , EnvId                     INT                                 NOT NULL                                    -- id_env_key
    , ModifiedUserId            INT                                 NOT NULL                                    -- id_user_mod_key
    , ModifiedOn                DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- dtt_mod
    , ValidFrom                 DATETIME2                           NOT NULL                    DEFAULT GETDATE()  -- dtt_valid_from
    , ValidTo                   DATETIME2                           NOT NULL                    DEFAULT DATEADD(year, 1, GETDATE())  -- dtt_valid_to
    
    -- Relationship mapping fields
    , FromTypeId                INT                                 NOT NULL                                    -- id_from_type_key
    , FromId                    INT                                 NOT NULL                                    -- id_from_key
    , FromVersion               INT                                 NOT NULL                                    -- id_from_key_ver
    , ToTypeId                  INT                                 NOT NULL                                    -- id_to_type_key
    , ToId                      INT                                 NOT NULL                                    -- id_to_key
    , ToVersion                 INT                                 NOT NULL                                    -- id_to_key_ver
    
    -- Relationship properties
    , IsPrimary                 INT                                 NOT NULL                                    -- is_primary
    , SortOrder                 INT                                 NOT NULL                                    -- ct_sort_order
    , Description               NVARCHAR(2048)                      NOT NULL                                    -- tx_desc
    
    -- Primary key constraint
    CONSTRAINT [pk_generic_map] PRIMARY KEY CLUSTERED 
    (
        [GenericMapId] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
) ON [PRIMARY]
GO