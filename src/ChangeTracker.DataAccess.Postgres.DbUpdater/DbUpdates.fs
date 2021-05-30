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
    { Version = 3; Update = UserTable.create }
    { Version = 4; Update = UserTable.addUserForDefaultVersioningSchemes}
    { Version = 5; Update = VersioningSchemeTable.create }
    { Version = 6; Update = AccountTable.addVersioningSchemeForeignKey }
    { Version = 7; Update = VersioningSchemeTable.addSemVer2DefaultScheme }
    { Version = 8; Update = RoleTable.create }
    { Version = 9; Update = RolePermissionTable.create } 
    { Version = 10; Update = ApiKeyTable.create }
    { Version = 11; Update = AccountUserTable.create }
    { Version = 12; Update = ProductTable.create }
    { Version = 13; Update = ProductTable.addUniqueIndexOnAccountIdAndName}
    { Version = 14; Update = ProductUserTable.create}
    { Version = 15; Update = VersionTable.create}
    { Version = 16; Update = VersionTable.addUniqueIndexProductIdValueDeletedAt}
    { Version = 17; Update = ChangeLogLineTable.create}
    { Version = 18; Update = ChangeLogLineTable.addPartialUniqueIndexOnProductIdVersionIdTextDeletedAt}
    { Version = 19; Update = Functions.createGuidFunction}
    { Version = 20; Update = RoleTable.addBasicRoles}
    { Version = 21; Update = VersioningSchemeTable.addUniqueIndexOnNameAccountIdDeletedAt}
    { Version = 22; Update = RolePermissionTable.addPermissionAddOrUpdateProduct}
    { Version = 23; Update = RolePermissionTable.addPermissionViewChangeLogLines}
    { Version = 24; Update = RolePermissionTable.addSomeViewPermissions}
    { Version = 25; Update = AccountViews.createAccountUserRolesView}
    { Version = 26; Update = ProductViews.createProductUserRolesView}
    { Version = 27; Update = CommonFunctions.addGuidFunction}
    { Version = 28; Update = UserTable.fixEmailUniqeConstraint}
    { Version = 29; Update = VersionTable.addTextSearch}
    { Version = 30; Update = AccountUserTable.createIndexOnUserIdAndRoleId}
    { Version = 31; Update = RolePermissionTable.addVersionPermissions}
    { Version = 32; Update = UserTable.addUserForAppChanges}
    { Version = 33; Update = ProductTable.addProductForAppChanges}
    { Version = 34; Update = ProductTable.addProductForAppChanges}
    { Version = 35; Update = RolePermissionTable.addChangeLogLinePermissions}]
