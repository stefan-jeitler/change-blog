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

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createApiKeySql) |> ignore
