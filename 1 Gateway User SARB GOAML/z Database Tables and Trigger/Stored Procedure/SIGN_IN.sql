
DECLARE @_tx_login_name NVARCHAR(255) = 'user@example.com';  -- Input parameter
DECLARE @_tx_password NVARCHAR(512) = 'password123';         -- Input parameter
DECLARE @_rs_out NVARCHAR(MAX);                              -- Output parameter

-- SIGN_IN Action Implementation
IF @_tx_action_name = 'SIGN_IN'

BEGIN
	SELECT @_rs_out = (
		SELECT
			U.UserId AS userId,
            U.FirstName AS firstName,
            U.LastName AS lastName,
            U.LoginName AS loginName,
			(
				SELECT
				G.GroupId AS groupId,
                G.GroupName AS groupName,
				(
					SELECT
					R.RoleId AS roleId,
                    R.RoleName AS roleName,
					(
						SELECT
						P.PermissionId AS permissionId,
                        TV.TypeValue AS permissionType,
                        P.PermissionName AS permissionName
						FROM [GenericMap] M3
                        INNER JOIN [Permission] P ON P.PermissionId = M3.ToId
                        INNER JOIN [TypeValue] TV ON TV.TypeValueId = P.PermissionTypeId
						WHERE M3.FromId = R.RoleId
                                  AND M3.FromTypeName = 'ROLE'
                                  AND M3.ToTypeName = 'PERMISSION'
                                  AND M3.IsActive = 1
						FOR JSON PATH
					) AS [permission]
					FROM [GenericMap] M2
                        INNER JOIN [Role] R ON R.RoleId = M2.ToId
                        WHERE M2.FromId = G.GroupId
                          AND M2.FromTypeName = 'GROUP'
                          AND M2.ToTypeName = 'ROLE'
                          AND M2.IsActive = 1
                        FOR JSON PATH
				)AS [role]
				FROM [GenericMap] M1
                INNER JOIN [Group] G ON G.GroupId = M1.ToId
                WHERE M1.FromId = U.UserId
                  AND M1.FromTypeName = 'USER'
                  AND M1.ToTypeName = 'GROUP'
                  AND M1.IsActive = 1
                FOR JSON PATH
			) AS groups
			FROM [User] U
        WHERE U.IsActive = 1
          AND U.LoginName = @_tx_login_name
          AND U.Password = HASHBYTES('SHA2_256', @_tx_password + CAST(U.UserId AS NVARCHAR(50))) -- Simple password hash example
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
	);
		IF @_rs_out = '[]' OR @_rs_out IS NULL
        SET @_rs_out = NULL;
END