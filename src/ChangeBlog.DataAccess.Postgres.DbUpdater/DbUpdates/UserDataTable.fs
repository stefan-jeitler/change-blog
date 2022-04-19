module UserDataTable

open System.Data
open Dapper

let private createSql = """
        CREATE TABLE IF NOT EXISTS user_data (
        user_id UUID PRIMARY KEY,
        data JSONB CONSTRAINT userdata_data_nn NOT NULL,
        updated_at TIMESTAMP CONSTRAINT userdata_updatedat_nn NOT NULL,
        CONSTRAINT userdata_userid_fkey FOREIGN KEY (user_id) REFERENCES "user"(id)
        )
    """
    
let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createSql)
    |> ignore