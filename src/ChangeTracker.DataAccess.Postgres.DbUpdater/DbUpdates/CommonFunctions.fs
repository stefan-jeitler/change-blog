module CommonFunctions

open System.Data
open Dapper

let private addGuidFunctionSql = """
        create or replace function guid() returns uuid
        language sql
        as
        $$
        select uuid_generate_v4();
        $$
    """

let addGuidFunction (dbConnection: IDbConnection) =
    dbConnection.Execute(addGuidFunctionSql)
    |> ignore