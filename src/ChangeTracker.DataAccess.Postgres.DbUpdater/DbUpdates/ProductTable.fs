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

let private addUniqueIndexOnAccountIdAndNameSql = "CREATE UNIQUE INDEX IF NOT EXISTS product_accountid_name_unique ON product (account_id, LOWER(name))"

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createProductSql)
    |> Async.AwaitTask
    |> Async.Ignore

let addUniqueIndexOnAccountIdAndName (dbConnection: IDbConnection) = 
    dbConnection.ExecuteAsync(addUniqueIndexOnAccountIdAndNameSql)
    |> Async.AwaitTask
    |> Async.Ignore    
