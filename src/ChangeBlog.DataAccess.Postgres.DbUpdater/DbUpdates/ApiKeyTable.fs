module ApiKeyTable

open System.Data
open Dapper

let private createApiKeySql =
    """
        CREATE TABLE IF NOT EXISTS api_key
        (
        	id UUID CONSTRAINT apikey_id_pkey PRIMARY KEY,
        	user_id UUID CONSTRAINT apikey_userid_nn NOT NULL,
        	"key" TEXT CONSTRAINT apikey_key_nn NOT NULL,
        	expires_at TIMESTAMP CONSTRAINT apikey_expiresat_nn NOT NULL,
        	deleted_at TIMESTAMP,
        	created_at TIMESTAMP CONSTRAINT apikey_createdat_nn NOT NULL,
        	CONSTRAINT apikey_userid_fkey FOREIGN KEY(user_id) REFERENCES "user"(id),
        	CONSTRAINT apikey_key_unique UNIQUE ("key")
        )
    """
    
let private addTitleColumnSql = [
    "ALTER TABLE api_key ADD COLUMN IF NOT EXISTS title TEXT"
    "UPDATE api_key SET title = '' WHERE title IS NULL"
    "ALTER TABLE api_key ALTER COLUMN title SET NOT NULL"
]

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createApiKeySql) |> ignore

let addTitleColumn (dbConnection: IDbConnection) =
    addTitleColumnSql
    |> List.map dbConnection.Execute
    |> ignore