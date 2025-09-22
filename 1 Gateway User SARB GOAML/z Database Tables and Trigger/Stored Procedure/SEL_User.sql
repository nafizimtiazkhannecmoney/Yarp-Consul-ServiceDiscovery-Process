/**
-- =============================================
-- Author:      Md. Mahbub Hasan Mohiuddin
-- Create Date: 10/08/2025
-- Version: 0.0.1
-- Description: Returns the User information.
-- =============================================
*/
DROP PROCEDURE IF EXISTS [dbo].[SEL_user]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

    CREATE PROCEDURE [dbo].[SEL_user]
        -- Add the parameters for the stored procedure here
        @jsonData NVARCHAR(MAX)
    AS   
    BEGIN
            
        DECLARE
                @g_ct_row			INT
            ,   @loginName		    VARCHAR(128) = JSON_VALUE(@jsonData, '$.LoginName')
            ,   @password		    VARCHAR(256) = JSON_VALUE(@jsonData, '$.Password')
            ,   @UserId			    INT 		 = NULL;
            ,   @LegalEntityId	    INT 		 = NULL;
            ,   @OldPassword		VARCHAR(256) = NULL;
            ,   @ActionName		    VARCHAR(128) = NULL;



        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

        -- Statements for procedure here
        SELECT U.*
        FROM [dbo].[User] U
        WHERE U.LoginName = @loginName
        AND U.Password = @password
        FOR JSON AUTO;

        SET NOCOUNT OFF;
    END
GO