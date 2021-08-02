module ProductUserTable

open System.Data
open Dapper

let private createProductUserRoleSql =
    """
		CREATE TABLE IF NOT EXISTS product_user
		(
			product_id UUID,
			user_id UUID,
			role_id UUID,
			created_at TIMESTAMP CONSTRAINT productuserrole_createdat_nn NOT NULL,
			CONSTRAINT productuserrole_userid_productid_roleid_pkey PRIMARY KEY (product_id, user_id, role_id),
			CONSTRAINT productuserrole_userid_fkey FOREIGN KEY (user_id) REFERENCES "user"(id),
			CONSTRAINT productuserrole_productid_fkey FOREIGN KEY (product_id) REFERENCES product(id),
			CONSTRAINT productuserrole_roleid_fkey FOREIGN KEY (role_id) REFERENCES "role"(id)
		)
    """

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createProductUserRoleSql)
    |> ignore
