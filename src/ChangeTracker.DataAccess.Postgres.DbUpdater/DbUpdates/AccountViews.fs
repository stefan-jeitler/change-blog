module AccountViews

open System.Data
open Dapper

let private createAccountUserRolesViewSql = """
    CREATE OR REPLACE VIEW account_user_roles AS
    select au.account_id,
           (select name as account_name from account where id = au.account_id) as account_name,
           u.id   as user_id,
           u.email,
           r.name as role
    from "user" u
             join account_user au on u.id = au.user_id
             join role r on au.role_id = r.id
    order by 2
    """

let createAccountUserRolesView (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createAccountUserRolesViewSql)
    |> Async.AwaitTask
    |> Async.Ignore
