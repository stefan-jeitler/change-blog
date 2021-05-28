module RolePermissionTable

open System.Data
open Dapper

let private createRolePermissionSql = """
		CREATE TABLE IF NOT EXISTS role_permission
		(
			role_id UUID,
			permission TEXT,
			created_at TIMESTAMP CONSTRAINT rolepermission_createdat_nn NOT NULL,
			CONSTRAINT rolepermission_roleid_permission_pkey PRIMARY KEY (role_id, permission),
			CONSTRAINT rolepermission_roleid_fkey FOREIGN KEY (role_id) REFERENCES "role"(id)
		)
    """

let private addPermissionAddProductSql = """
		INSERT INTO role_permission
		SELECT id, 'AddProduct', now() from role where name in ('ProductOwner', 'PlatformManager', 'Developer')
		ON CONFLICT (role_id, permission) DO NOTHING
	"""

let private addPermissionViewChangeLogLinesSql = """
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
  	  SELECT id, 'AddVersion', now() from role where name in ('ScrumMaster', 'ProductOwner', 'PlatformManager', 'Developer')
  	  ON CONFLICT (role_id, permission) DO NOTHING
	"""
      """
  	  INSERT INTO role_permission
  	  SELECT id, 'ViewCompleteVersion', now() from role where name in ('Support', 'ScrumMaster', 'ProductOwner', 'ProductManager', 'PlatformManager', 'Developer')
  	  ON CONFLICT (role_id, permission) DO NOTHING
	""" ]

let private addVersionPermissionsSql = [
    """
	  INSERT INTO role_permission
	  SELECT id, 'ReleaseVersion', now() from role where name in ('ScrumMaster', 'ProductOwner', 'PlatformManager', 'Developer')
	  ON CONFLICT (role_id, permission) DO NOTHING
   """
    """
	  INSERT INTO role_permission
	  SELECT id, 'DeleteVersion', now() from role where name in ('ProductOwner', 'PlatformManager', 'Developer')
	  ON CONFLICT (role_id, permission) DO NOTHING
   """
    """
	  INSERT INTO role_permission
	  SELECT id, 'UpdateVersion', now() from role where name in ('ProductOwner', 'PlatformManager', 'Developer')
	  ON CONFLICT (role_id, permission) DO NOTHING
   """
]

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createRolePermissionSql)
    |> ignore

let addPermissionAddProduct (dbConnection: IDbConnection) =
    dbConnection.Execute(addPermissionAddProductSql)
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
            insertPermissions tail |> ignore

    insertPermissions addSomeViewPermissionsSql

let addVersionPermissions (dbConnection: IDbConnection) = 
    addVersionPermissionsSql
    |> List.map (fun x -> dbConnection.Execute(x))
    |> ignore

    ()