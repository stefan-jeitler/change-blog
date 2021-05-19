module RoleTable

open System.Data
open Dapper

let private createRoleSql = """
        CREATE TABLE IF NOT EXISTS "role"
        (
        	id UUID CONSTRAINT role_id_pkey PRIMARY KEY,
        	"name" TEXT CONSTRAINT role_name_nn NOT NULL,
        	description TEXT CONSTRAINT role_description_nn NOT NULL,
        	created_at TIMESTAMP CONSTRAINT role_createdat_nn NOT NULL,
        	CONSTRAINT role_name_unique UNIQUE ("name")
        )
    """

let private basicRolesInsertSql =
    [ """
        INSERT INTO "role"
        VALUES ('8ec8ae99-83d7-4958-9df5-72eb8eaf002b', 'DefaultUser', 'Basic user ', now())
        ON CONFLICT (id) DO NOTHING
    """
      """
        INSERT INTO "role"
        VALUES ('21bdebe9-4647-4610-b976-d5ace4ba1a7e', 'Support', 'Interested in changes', now())
        ON CONFLICT (id) DO NOTHING
    """
      """
        INSERT INTO "role"
        VALUES ('55821c64-7991-48ea-bc68-bcb0574e4ad4', 'ScrumMaster', 'Scrum master', now())
        ON CONFLICT (id) DO NOTHING
    """
      """
        INSERT INTO "role"
        VALUES ('a1288586-26ac-43e1-a20e-6cd6f678ac85', 'ProductOwner', 'Responsible for products', now())
        ON CONFLICT (id) DO NOTHING
    """
      """
        INSERT INTO "role"
        VALUES ('25298d8c-22b5-4ed4-92ba-13e46fe0561d', 'ProductManager', 'Interested in releases and its changelogs', now())
        ON CONFLICT (id) DO NOTHING
    """
      """
        INSERT INTO "role"
        VALUES ('446bceb2-0b9d-4899-934d-51be0576b7fa', 'PlatformManager', 'Maintains the account', now())
        ON CONFLICT (id) DO NOTHING
    """
      """
        INSERT INTO "role"
        VALUES ('ad7b83ed-8fce-4341-978b-8d1eae66f346', 'Developer', 'Commits versions and changelogs', now())
        ON CONFLICT (id) DO NOTHING
    """ ]

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createRoleSql)
    |> Async.AwaitTask
    |> Async.Ignore

let addBasicRoles (dbConnection: IDbConnection) =
    let rec insertRoles (roles: string list) =
        async {
            match roles with
            | [] -> ()
            | head :: tail ->
                do!
                    dbConnection.ExecuteAsync(head)
                    |> Async.AwaitTask
                    |> Async.Ignore

                do! insertRoles tail
        }

    insertRoles basicRolesInsertSql
