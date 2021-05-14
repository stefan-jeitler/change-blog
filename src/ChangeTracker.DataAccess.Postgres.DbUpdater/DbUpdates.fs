module DbUpdates

open System.Data

type DbUpdate = { 
    Version: int
    Update: IDbConnection -> Async<unit> 
}

let dbUpdates: DbUpdate list = [
    { Version = 0; Update = SchemaTable.create }
    { Version = 1; Update = AccountTable.create }
    { Version = 2; Update = VersioningSchemeTable.create }
    { Version = 3; Update = AccountTable.addVersioningSchemeForeignKey }
    { Version = 4; Update = RoleTable.create }
    { Version = 5; Update = VersioningSchemeTable.addSemVer2DefaultScheme }
    { Version = 6; Update = RolePermissionTable.create } 
    { Version = 7; Update = UserTable.create }
    { Version = 8; Update = ApiKeyTable.create }
    { Version = 9; Update = AccountUserTable.create }
    { Version = 10; Update = ProjectTable.create }
    { Version = 11; Update = ProjectUser.create}
    { Version = 12; Update = VersionTable.create}
    { Version = 13; Update = ChangeLogLineTable.create}
    { Version = 14; Update = Functions.createGuidFunction}
    { Version = 15; Update = RoleTable.addBasicRoles}
    { Version = 16; Update = VersionTable.modifyUniqueConstraint}
    { Version = 17; Update = ProjectTable.addUniqueIndexOnAccountIdAndName}
    { Version = 18; Update = AccountTable.fixUniqeNameConstraint}
    { Version = 19; Update = ChangeLogLineTable.fixUniqueIndexConstraint}
    { Version = 20; Update = VersioningSchemeTable.addUniquIndex}
    { Version = 21; Update = RolePermissionTable.addPermissionAddProject}
    { Version = 22; Update = RolePermissionTable.addPermissionViewChangeLogLines}
    { Version = 23; Update = RolePermissionTable.addSomeViewPermissions}]
