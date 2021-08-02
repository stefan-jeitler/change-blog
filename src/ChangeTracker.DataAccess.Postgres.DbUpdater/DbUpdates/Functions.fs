module Functions

open System.Data
open Dapper


let private createGuidFunctionSql =
    """
        CREATE OR REPLACE FUNCTION GUID() RETURNS uuid
        AS 'select uuid_generate_v4();'
        LANGUAGE SQL
    """

let createGuidFunction (dbConnection: IDbConnection) =
    dbConnection.Execute(createGuidFunctionSql)
    |> ignore
