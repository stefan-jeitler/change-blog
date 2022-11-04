module RolePermissionTable

open System.Data
open Dapper

let private createRolePermissionSql =
    """
		CREATE TABLE IF NOT EXISTS role_permission
		(
			role_id UUID,
			permission TEXT,
			created_at TIMESTAMP CONSTRAINT rolepermission_createdat_nn NOT NULL,
			CONSTRAINT rolepermission_roleid_permission_pkey PRIMARY KEY (role_id, permission),
			CONSTRAINT rolepermission_roleid_fkey FOREIGN KEY (role_id) REFERENCES "role"(id)
		)
    """

let private addPermissionAddOrUpdateProductSql =
    """
		INSERT INTO role_permission
		SELECT id, 'AddOrUpdateProduct', now() from role where name in ('ProductOwner', 'PlatformManager', 'Developer')
		ON CONFLICT (role_id, permission) DO NOTHING
	"""

let private addPermissionViewChangeLogLinesSql =
    """
		INSERT INTO role_permission
		SELECT id, 'ViewChangeLogLines', now() FROM role where name <> 'DefaultUser'
		ON CONFLICT (role_id, permission) DO NOTHING
	"""

let private addSomeViewPermissionsSql =
    [ """
      INSERT INTO role_permission
      SELECT id, 'CloseProduct', now() from role where name in ('ProductOwner', 'PlatformManager')
      ON CONFLICT (role_id, permission) DO NOTHING
    """
      """
      INSERT INTO role_permission
      SELECT id, 'ViewAccountProducts', now() from role where name in ('Support', 'ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
      ON CONFLICT (role_id, permission) DO NOTHING
    """
      """
      INSERT INTO role_permission
      SELECT id, 'ViewRoles', now() from role where name in ('ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
      ON CONFLICT (role_id, permission) DO NOTHING
    """
      """
      INSERT INTO role_permission
      SELECT id, 'ViewAccount', now() from role where name in ('DefaultUser', 'Support', 'ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
      ON CONFLICT (role_id, permission) DO NOTHING
    """
      """
      INSERT INTO role_permission
      SELECT id, 'ViewAccountUsers', now() from role where name in ('PlatformManager', 'ProductOwner')
      ON CONFLICT (role_id, permission) DO NOTHING
    """
      """
  	  INSERT INTO role_permission
  	  SELECT id, 'ViewUserProducts', now() from role where name in ('Support', 'ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
  	  ON CONFLICT (role_id, permission) DO NOTHING
	"""
      """
  	  INSERT INTO role_permission
  	  SELECT id, 'ViewOwnUser', now() from role where name in ('DefaultUser', 'Support', 'ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
  	  ON CONFLICT (role_id, permission) DO NOTHING
	"""
      """
  	  INSERT INTO role_permission
  	  SELECT id, 'AddOrUpdateVersion', now() from role where name in ('ScrumMaster', 'ProductOwner', 'PlatformManager', 'Developer')
  	  ON CONFLICT (role_id, permission) DO NOTHING
	"""
      """
 	  INSERT INTO role_permission
 	  SELECT id, 'AddVersion', now() from role where name in ('ScrumMaster', 'ProductOwner', 'PlatformManager', 'Developer')
 	  ON CONFLICT (role_id, permission) DO NOTHING
     """
      """
  	  INSERT INTO role_permission
  	  SELECT id, 'ViewVersions', now() from role where name in ('Support', 'ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
  	  ON CONFLICT (role_id, permission) DO NOTHING
	""" ]

let private addVersionPermissionsSql =
    [ """
	  INSERT INTO role_permission
	  SELECT id, 'ReleaseVersion', now() from role where name in ('ScrumMaster', 'ProductOwner', 'PlatformManager', 'Developer')
	  ON CONFLICT (role_id, permission) DO NOTHING
   """
      """
	  INSERT INTO role_permission
	  SELECT id, 'DeleteVersion', now() from role where name in ('ProductOwner', 'PlatformManager', 'Developer')
	  ON CONFLICT (role_id, permission) DO NOTHING
   """ ]

let private addChangeLogLinesPermissionSql =
    [ """
    INSERT INTO role_permission
    SELECT id, 'ViewPendingChangeLogLines', now() from role where name in ('ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
    ON CONFLICT (role_id, permission) DO NOTHING
   """
      """
    INSERT INTO role_permission
    SELECT id, 'AddOrUpdateChangeLogLine', now() from role where name in ('ScrumMaster', 'ProductOwner', 'PlatformManager', 'Developer')
    ON CONFLICT (role_id, permission) DO NOTHING
   """
      """
    INSERT INTO role_permission
    SELECT id, 'DeleteChangeLogLine', now() from role where name in ('ProductOwner', 'PlatformManager', 'Developer')
    ON CONFLICT (role_id, permission) DO NOTHING
   """
      """
    INSERT INTO role_permission
    SELECT id, 'MoveChangeLogLines', now() from role where name in ('ProductOwner', 'PlatformManager', 'Developer')
    ON CONFLICT (role_id, permission) DO NOTHING
   """ ]

let private deleteObsoletePermissionsSql =
    """
        delete
        from role_permission
        where permission in ('ViewOwnUser', 'ViewRoles', 'ViewUserProducts')
    """

let private addAccountPermissionsSql =
    [ """
    INSERT INTO role_permission
    SELECT id, 'UpdateAccount', now() from role where name in ('PlatformManager')
    ON CONFLICT (role_id, permission) DO NOTHING
    """
      """
    INSERT INTO role_permission
    SELECT id, 'DeleteAccount', now() from role where name in ('PlatformManager')
    ON CONFLICT (role_id, permission) DO NOTHING
    """ ]

let private addNewViewAndFreezeProductPermissionsSql =
    [ """
    INSERT INTO role_permission
    SELECT id, 'FreezeProduct', now() from role where name in ('ProductOwner', 'PlatformManager')
    ON CONFLICT (role_id, permission) DO NOTHING
	"""
      """
	INSERT INTO role_permission
	SELECT id, 'ViewProduct', now() from role where name in ('Support', 'ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
	ON CONFLICT (role_id, permission) DO NOTHING
    """ ]

let private deleteViewAccountProductsAndCloseProductAndAddVersionPermissionsSql =
    """
        delete
        from role_permission
        where permission in ('CloseProduct', 'ViewAccountProducts', 'AddVersion')
	"""

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createRolePermissionSql)
    |> ignore

let addPermissionAddOrUpdateProduct (dbConnection: IDbConnection) =
    dbConnection.Execute(addPermissionAddOrUpdateProductSql)
    |> ignore

let addPermissionViewChangeLogLines (dbConnection: IDbConnection) =
    dbConnection.Execute(addPermissionViewChangeLogLinesSql)
    |> ignore

let addSomeViewPermissions (dbConnection: IDbConnection) =
    let rec insertPermissions (permissions: string list) =
        match permissions with
        | [] -> ()
        | head :: tail ->
            dbConnection.Execute(head) |> ignore
            insertPermissions tail

    insertPermissions addSomeViewPermissionsSql

let addVersionPermissions (dbConnection: IDbConnection) =
    addVersionPermissionsSql
    |> List.map dbConnection.Execute
    |> ignore

let addChangeLogLinePermissions (dbConnection: IDbConnection) =
    addChangeLogLinesPermissionSql
    |> List.map dbConnection.Execute
    |> ignore

let deleteObsoletePermissions (dbConnection: IDbConnection) =
    dbConnection.Execute(deleteObsoletePermissionsSql)
    |> ignore

let addAccountPermissions (dbConnection: IDbConnection) =
    addAccountPermissionsSql
    |> List.map dbConnection.Execute
    |> ignore

let alignWithAppPermissions (dbConnection: IDbConnection) =
    addNewViewAndFreezeProductPermissionsSql
    |> List.map dbConnection.Execute
    |> ignore

    dbConnection.Execute(deleteViewAccountProductsAndCloseProductAndAddVersionPermissionsSql)
    |> ignore
