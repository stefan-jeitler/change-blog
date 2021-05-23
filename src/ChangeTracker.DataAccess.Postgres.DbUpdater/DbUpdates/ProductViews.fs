module ProductViews

open System.Data
open Dapper

let private createProductUserRolesViewSql = """
    CREATE OR REPLACE VIEW product_user_roles AS
    select
        p.account_id,
        (select name from account where id = p.account_id) as account_name,
        u.id as userId,
        u.email,
        r.name as role,
        p.name as projectName,
        p.id
    from "user" u
    join product_user pu on u.id = pu.user_id
    join product p on pu.product_id = p.id
    join role r on pu.role_id = r.id
    order by 2
"""

let createProductUserRolesView (dbConnection: IDbConnection) =
    dbConnection.Execute(createProductUserRolesViewSql)
    |> ignore
