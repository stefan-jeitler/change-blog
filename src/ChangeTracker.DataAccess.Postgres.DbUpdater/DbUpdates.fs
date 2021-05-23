module DbUpdates

open System.Data

type DbUpdate = { 
    Version: int
    Update: IDbConnection -> unit
}

let dbUpdates: DbUpdate list = [
    { Version = 0; Update = SchemaTable.create }
    { Version = 1; Update = AccountTable.create }
    { Version = 2; Update = AccountTable.addPartialUniqueIndexOnNameAndDeletedAt}
    { Version = 3; Update = VersioningSchemeTable.create }
    { Version = 4; Update = AccountTable.addVersioningSchemeForeignKey }
    { Version = 5; Update = VersioningSchemeTable.addSemVer2DefaultScheme }
    { Version = 6; Update = RoleTable.create }
    { Version = 7; Update = RolePermissionTable.create } 
    { Version = 8; Update = UserTable.create }
    { Version = 9; Update = ApiKeyTable.create }
    { Version = 10; Update = AccountUserTable.create }
    { Version = 11; Update = ProductTable.create }
    { Version = 12; Update = ProductTable.addUniqueIndexOnAccountIdAndName}
    { Version = 13; Update = ProductUserTable.create}
    { Version = 14; Update = VersionTable.create}
    { Version = 15; Update = VersionTable.addUniqueIndexProductIdValueDeletedAt}
    { Version = 16; Update = ChangeLogLineTable.create}
    { Version = 17; Update = ChangeLogLineTable.addPartialUniqueIndexOnProductIdVersionIdTextDeletedAt}
    { Version = 18; Update = Functions.createGuidFunction}
    { Version = 19; Update = RoleTable.addBasicRoles}
    { Version = 20; Update = VersioningSchemeTable.addUniqueIndexOnNameAccountIdDeletedAt}
    { Version = 21; Update = RolePermissionTable.addPermissionAddProduct}
    { Version = 22; Update = RolePermissionTable.addPermissionViewChangeLogLines}
    { Version = 23; Update = RolePermissionTable.addSomeViewPermissions}
    { Version = 24; Update = AccountViews.createAccountUserRolesView}
    { Version = 25; Update = ProductViews.createProductUserRolesView}
    { Version = 26; Update = ChangeLogLineTable.addTextSearch}]
