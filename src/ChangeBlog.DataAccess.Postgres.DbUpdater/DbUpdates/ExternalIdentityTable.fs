module ExternalIdentityTable

open System.Data
open Dapper


let private createTableSql = """
    CREATE TABLE IF NOT EXISTS external_identity
    (
        id UUID CONSTRAINT externalidentity_id_pkey PRIMARY KEY,
        user_id uuid CONSTRAINT externalidentity_userid_nn NOT NULL,
        external_user_id text CONSTRAINT externalidentity_userid_nn NOT NULL,
        identity_provider text CONSTRAINT externalidentity_identityprovider_nn NOT NULL,
        created_at timestamp CONSTRAINT externalidentity_createdat_nn NOT NULL,
        CONSTRAINT externalidentity_userid_fkey FOREIGN KEY (user_id) REFERENCES "user"(id),
        CONSTRAINT externalidentity_userid_externaluserid_unique UNIQUE (user_id, external_user_id),
        CONSTRAINT externalidentity_externaluserid_unique UNIQUE (external_user_id)
    )
"""

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createTableSql) |> ignore