-- MS SQL Server version of the sel_user stored procedure
-- DROP PROCEDURE IF EXISTS sel_user;

CREATE OR ALTER PROCEDURE sel_user
    @rs_out NVARCHAR(MAX) OUTPUT,
    @json NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @tx_action_name NVARCHAR(255) = NULL;
    DECLARE @id_user_key BIGINT = NULL;
    DECLARE @id_legal_entity_key BIGINT = NULL;
    DECLARE @tx_login_name NVARCHAR(255) = NULL;
    DECLARE @tx_password NVARCHAR(255) = NULL;
    DECLARE @tx_old_password NVARCHAR(255) = NULL;
    DECLARE @id_env_key INT = NULL;
    
    -- Parse JSON input
    IF ISJSON(@json) = 1
    BEGIN
        SELECT @tx_action_name = JSON_VALUE(@json, '$.actionName');
        SELECT @id_user_key = TRY_CAST(JSON_VALUE(@json, '$.userId') AS BIGINT);
        SELECT @id_legal_entity_key = TRY_CAST(JSON_VALUE(@json, '$.legalEntityId') AS BIGINT);
        SELECT @tx_login_name = JSON_VALUE(@json, '$.loginName');
        SELECT @tx_password = JSON_VALUE(@json, '$.password');
        SELECT @tx_old_password = JSON_VALUE(@json, '$.oldPassword');
    END
    ELSE
    BEGIN
        RAISERROR('Invalid JSON input', 16, 1);
        RETURN;
    END
    
    IF @tx_action_name IS NULL
    BEGIN
        RAISERROR('Action cannot be null', 16, 1);
        RETURN;
    END
    
    -------------------------------------------------------------
    IF @tx_action_name = 'GET_ALL_ROLES'
    BEGIN
        SELECT @rs_out = (
            SELECT 
                id_role_key AS id,
                id_role_key AS roleId,
                tx_role_name AS roleName
            FROM T_ROLE
            WHERE LOWER(tx_role_name) != LOWER('System')
            AND LOWER(tx_role_name) != LOWER('SA')
            AND LOWER(tx_role_name) != LOWER('TECH_ADMIN')
            FOR JSON AUTO
        );
    END
    -------------------------------------------------------------
    ELSE IF @tx_action_name = 'GET_ALL_USER'
    BEGIN
        SELECT @rs_out = (
            SELECT 
                U.id_user_key AS userId,
                U.tx_phone AS phone,
                U.tx_first_name AS firstName,
                U.tx_last_name AS lastName,
                U.tx_login_name AS loginName,
                U.j_user_info AS userInfo
            FROM T_USER U
            WHERE U.is_active = 1
            AND U.tx_login_name != 'sa@necmoney.co.za'
            FOR JSON AUTO
        );
    END
    -------------------------------------------------------------
    ELSE IF @tx_action_name = 'GET_USER_BY_ID'
    BEGIN
        SELECT @rs_out = (
            SELECT 
                U.id_user_key AS userId,
                U.tx_phone AS phone,
                U.tx_first_name AS firstName,
                U.tx_last_name AS lastName,
                U.tx_login_name AS loginName,
                U.j_user_info AS userInfo,
                (
                    SELECT 
                        G.id_group_key AS groupId,
                        G.tx_group_name AS groupName
                    FROM V_GENERIC_MAP M1
                    INNER JOIN T_GROUP G ON G.id_group_key = M1.id_to_key
                    WHERE M1.id_from_key = U.id_user_key
                    AND M1.tx_from_type_name = 'USER'
                    AND M1.tx_to_type_name = 'GROUP'
                    FOR JSON AUTO
                ) AS groups,
                (
                    SELECT 
                        G.id_group_key AS roleId,
                        G.tx_group_name AS roleName
                    FROM V_GENERIC_MAP M1
                    INNER JOIN T_GROUP G ON G.id_group_key = M1.id_to_key
                    WHERE M1.id_from_key = U.id_user_key
                    AND M1.tx_from_type_name = 'USER'
                    AND M1.tx_to_type_name = 'GROUP'
                    FOR JSON AUTO
                ) AS roles
            FROM T_USER U
            WHERE U.is_active = 1
            AND U.id_user_key = @id_user_key
            FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER
        );
    END
    -------------------------------------------------------------
    ELSE IF @tx_action_name = 'SIGN_IN'
    BEGIN
        SELECT @rs_out = (
            SELECT 
                U.id_user_key AS userId,
                U.tx_first_name AS firstName,
                U.tx_last_name AS lastName,
                U.tx_login_name AS loginName,
                (
                    SELECT 
                        G.id_group_key AS groupId,
                        G.tx_group_name AS groupName,
                        (
                            SELECT 
                                R.id_role_key AS roleId,
                                R.tx_role_name AS roleName,
                                (
                                    SELECT 
                                        P.id_permission_key AS permissionId,
                                        TV.tx_type_value AS permissionType,
                                        P.tx_permission_name AS permissionName
                                    FROM V_GENERIC_MAP M3
                                    INNER JOIN T_PERMISSION P ON P.id_permission_key = M3.id_to_key
                                    INNER JOIN T_TYPE_VALUE TV ON TV.id_type_value_key = P.id_permission_type
                                    WHERE M3.id_from_key = R.id_role_key
                                    AND M3.tx_from_type_name = 'ROLE'
                                    AND M3.tx_to_type_name = 'PERMISSION'
                                    FOR JSON AUTO
                                ) AS permission
                            FROM V_GENERIC_MAP M2
                            INNER JOIN T_ROLE R ON R.id_role_key = M2.id_to_key
                            WHERE M2.id_from_key = G.id_group_key
                            AND M2.tx_from_type_name = 'GROUP'
                            AND M2.tx_to_type_name = 'ROLE'
                            FOR JSON AUTO
                        ) AS role
                    FROM V_GENERIC_MAP M1
                    INNER JOIN T_GROUP G ON G.id_group_key = M1.id_to_key
                    WHERE M1.id_from_key = U.id_user_key
                    AND M1.tx_from_type_name = 'USER'
                    AND M1.tx_to_type_name = 'GROUP'
                    FOR JSON AUTO
                ) AS groups
            FROM T_USER U
            WHERE U.is_active = 1
            AND U.tx_login_name = @tx_login_name
            AND U.tx_password = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @tx_password + U.tx_password), 2)
            FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER
        );
    END
    -------------------------------------------------------------
    ELSE IF @tx_action_name = 'GET_BY_KEY'
    BEGIN
        SELECT @rs_out = (
            SELECT 
                U.id_user_key AS userId,
                U.tx_phone AS phone,
                U.tx_first_name AS firstName,
                U.tx_last_name AS lastName,
                U.tx_login_name AS loginName,
                U.j_user_info AS userInfo
            FROM T_USER U
            WHERE U.is_active = 1
            AND JSON_VALUE(U.j_user_info, '$.apiKey') = @tx_password
            FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER
        );
    END
    -------------------------------------------------------------
    ELSE IF @tx_action_name = 'SELECT_EXIST'
    BEGIN
        IF @tx_old_password IS NULL
        BEGIN
            SELECT @rs_out = (
                SELECT 
                    U.id_user_key,
                    U.id_user_ver,
                    U.tx_password,
                    CONCAT(U.tx_first_name, ' ', U.tx_last_name) AS tx_first_name
                FROM T_USER U
                WHERE U.is_active = 1
                AND U.tx_login_name = @tx_login_name
                FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER
            );
        END
        ELSE
        BEGIN
            SELECT @rs_out = (
                SELECT 
                    U.id_user_key,
                    U.id_user_ver,
                    U.tx_password,
                    CONCAT(U.tx_first_name, ' ', U.tx_last_name) AS tx_first_name
                FROM T_USER U
                WHERE U.is_active = 1
                AND U.tx_login_name = @tx_login_name
                AND U.tx_password = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @tx_old_password + U.tx_password), 2)
                FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER
            );
        END
    END
    -------------------------------------------------------------
    ELSE IF @tx_action_name = 'GET_ROLES'
    BEGIN
        SELECT @rs_out = (
            SELECT (
                SELECT 
                    id_role_key AS id,
                    id_role_key AS roleId,
                    tx_role_name AS roleName
                FROM T_ROLE
                WHERE LOWER(tx_role_name) != LOWER('System')
                AND LOWER(tx_role_name) != LOWER('SA')
                AND LOWER(tx_role_name) != LOWER('TECH_ADMIN')
                FOR JSON AUTO
            ) AS roles
            FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER
        );
    END
    -------------------------------------------------------------
    ELSE IF @tx_action_name = 'LOGIN'
    BEGIN
        SELECT @rs_out = (
            SELECT 
                U.id_user_key,
                U.id_user_ver,
                U.is_allow_login,
                U.id_legal_entity_key,
                U.dtt_mod,
                U.tx_login_name,
                U.id_user_mod_key,
                U.id_env_key,
                U.tx_first_name,
                U.tx_last_name,
                U.tx_phone,
                U.tx_email,
                U.j_user_info,
                (
                    SELECT 
                        G.id_group_key,
                        G.tx_group_name,
                        G.tx_desc,
                        (
                            SELECT 
                                R.id_role_key,
                                R.tx_role_name,
                                R.tx_desc,
                                (
                                    SELECT 
                                        P.id_permission_key,
                                        TV.tx_type_value,
                                        P.tx_permission_name,
                                        P.tx_desc
                                    FROM V_GENERIC_MAP M3
                                    INNER JOIN T_PERMISSION P ON P.id_permission_key = M3.id_to_key
                                    INNER JOIN T_TYPE_VALUE TV ON TV.id_type_value_key = P.id_permission_type
                                    WHERE M3.id_from_key = R.id_role_key
                                    AND M3.tx_from_type_name = 'ROLE'
                                    AND M3.tx_to_type_name = 'PERMISSION'
                                    FOR JSON AUTO
                                ) AS permission
                            FROM V_GENERIC_MAP M2
                            INNER JOIN T_ROLE R ON R.id_role_key = M2.id_to_key
                            WHERE M2.id_from_key = G.id_group_key
                            AND M2.tx_from_type_name = 'GROUP'
                            AND M2.tx_to_type_name = 'ROLE'
                            FOR JSON AUTO
                        ) AS role
                    FROM V_GENERIC_MAP M1
                    INNER JOIN T_GROUP G ON G.id_group_key = M1.id_to_key
                    WHERE M1.id_from_key = U.id_user_key
                    AND M1.tx_from_type_name = 'USER'
                    AND M1.tx_to_type_name = 'GROUP'
                    FOR JSON AUTO
                ) AS [group]
            FROM T_USER U
            WHERE U.is_active = 1
            AND U.tx_login_name = @tx_login_name
            AND U.tx_password = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @tx_password + U.tx_password), 2)
            FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER
        );
        
        IF @rs_out IS NOT NULL
        BEGIN
            SELECT @id_user_key = JSON_VALUE(@rs_out, '$.id_user_key');
            
            IF EXISTS (
                SELECT 1 FROM T_LOGIN
                WHERE id_user_key = @id_user_key
            )
            BEGIN
                UPDATE T_LOGIN
                SET id_login_ver = id_login_ver + 1,
                    is_logged_in = 1,
                    dtt_login = GETDATE(),
                    dtt_logout = '9999-12-31 23:59:59.997',
                    tx_desc = 'USER LOGGED IN ON -> ' + CONVERT(VARCHAR, GETDATE(), 120)
                WHERE id_user_key = @id_user_key;
            END
            ELSE
            BEGIN
                INSERT INTO T_LOGIN (
                    id_login_ver,
                    id_env_key,
                    dtt_login,
                    dtt_logout,
                    id_user_key,
                    tx_client_ip_addr,
                    is_logged_in,
                    tx_desc
                )
                VALUES (
                    0,
                    ISNULL(@id_env_key, -9999),
                    GETDATE(),
                    '9999-12-31 23:59:59.997',
                    ISNULL(@id_user_key, -9999),
                    '?',
                    1,
                    'USER LOGGED IN ON -> ' + CONVERT(VARCHAR, GETDATE(), 120)
                );
            END
        END
    END
    -------------------------------------------------------------
    ELSE IF @tx_action_name = 'SELECT'
    BEGIN
        SELECT @rs_out = (
            SELECT 
                U.id_user_key AS id,
                U.id_user_key,
                U.id_user_ver,
                U.is_allow_login,
                U.id_legal_entity_key,
                U.dtt_mod,
                U.tx_login_name,
                U.id_user_mod_key,
                U.id_env_key,
                U.tx_first_name,
                U.tx_last_name,
                U.tx_phone,
                U.tx_email,
                U.j_user_info,
                (
                    SELECT 
                        G.id_group_key,
                        G.tx_group_name,
                        G.tx_desc,
                        (
                            SELECT 
                                R.id_role_key,
                                R.tx_role_name,
                                R.tx_desc,
                                (
                                    SELECT 
                                        P.id_permission_key,
                                        TV.tx_type_value,
                                        P.tx_permission_name,
                                        P.tx_desc
                                    FROM V_GENERIC_MAP M3
                                    INNER JOIN T_PERMISSION P ON P.id_permission_key = M3.id_to_key
                                    INNER JOIN T_TYPE_VALUE TV ON TV.id_type_value_key = P.id_permission_type
                                    WHERE M3.id_from_key = R.id_role_key
                                    AND M3.tx_from_type_name = 'ROLE'
                                    AND M3.tx_to_type_name = 'PERMISSION'
                                    FOR JSON AUTO
                                ) AS permission
                            FROM V_GENERIC_MAP M2
                            INNER JOIN T_ROLE R ON R.id_role_key = M2.id_to_key
                            WHERE M2.id_from_key = G.id_group_key
                            AND M2.tx_from_type_name = 'GROUP'
                            AND M2.tx_to_type_name = 'ROLE'
                            FOR JSON AUTO
                        ) AS role
                    FROM V_GENERIC_MAP M1
                    INNER JOIN T_GROUP G ON G.id_group_key = M1.id_to_key
                    WHERE M1.id_from_key = U.id_user_key
                    AND M1.tx_from_type_name = 'USER'
                    AND M1.tx_to_type_name = 'GROUP'
                    FOR JSON AUTO
                ) AS [group]
            FROM T_USER U
            WHERE U.is_active = 1
            AND U.tx_login_name != 'sa@necmoney.co.za'
            AND U.tx_login_name = ISNULL(@tx_login_name, U.tx_login_name)
            FOR JSON AUTO
        );
    END
    ELSE
    BEGIN
        PRINT 'Unknown action: ' + ISNULL(@tx_action_name, 'NULL');
    END
END