module ProductTable

open System.Data
open Dapper

let private createProductSql = """
        CREATE TABLE IF NOT EXISTS product
        (
        	id UUID CONSTRAINT product_id_pkey PRIMARY KEY,
        	account_id UUID CONSTRAINT product_accountid_nn NOT NULL,
        	versioning_scheme_id UUID CONSTRAINT product_versioningschemeid_nn NOT NULL,
        	"name" TEXT CONSTRAINT product_name_nn NOT NULL,
        	created_by_user UUID CONSTRAINT product_createdbyuser_nn NOT NULL,
        	closed_at TIMESTAMP,
        	created_at TIMESTAMP CONSTRAINT product_createdat_nn NOT NULL,
        	CONSTRAINT product_accountid_fkey FOREIGN KEY (account_id) REFERENCES account(id),
        	CONSTRAINT product_versioningschemeid_fkey FOREIGN KEY (versioning_scheme_id) REFERENCES versioning_scheme(id),
        	CONSTRAINT product_createdbyuser_fkey FOREIGN KEY (created_by_user) REFERENCES "user"(id)
        )
    """

let private addUniqueIndexOnAccountIdAndNameSql =
    "CREATE UNIQUE INDEX IF NOT EXISTS product_accountid_name_unique ON product (account_id, LOWER(name))"

let private addProductForAppChangesSql = """
        INSERT INTO product
        VALUES ('5701bbed-83f5-4551-afaf-9ac546636473',
                'a00788cb-03f8-4a8c-84b6-756622550e8c',
                '4091b948-9bc5-43ee-9f98-df3d27853565',
                'ChangeTracker.Api',
                '8e983811-7d39-4fe2-9373-1c6b0e4eb360',
                null,
                '2021-05-29 10:45:44.737828')
        ON CONFLICT (id) do nothing 
    """

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createProductSql) |> ignore

let addUniqueIndexOnAccountIdAndName (dbConnection: IDbConnection) =
    dbConnection.Execute(addUniqueIndexOnAccountIdAndNameSql)
    |> ignore

    
let addProductForAppChanges (dbConnection: IDbConnection) =
    dbConnection.Execute(addProductForAppChangesSql)
    |> ignore
    