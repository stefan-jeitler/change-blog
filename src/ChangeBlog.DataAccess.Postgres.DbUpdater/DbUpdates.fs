module DbUpdates

open System.Data
open Semver

type DbUpdate =
    { Version: SemVersion
      Update: IDbConnection -> unit }
    
let private v x = SemVersion.Parse(x, SemVersionStyles.Strict)

let dbUpdates: DbUpdate list =
    [ { Version = v "1.0.0"; Update = SchemaTable.create }
      { Version = v "1.1.0"; Update = AccountTable.create }
      { Version = v "2.0.0"; Update = AccountTable.addPartialUniqueIndexOnNameAndDeletedAt }
      { Version = v "2.1.0"; Update = UserTable.create }
      { Version = v "2.2.0"; Update = UserTable.addUserForDefaultVersioningSchemes }
      { Version = v "2.3.0"; Update = VersioningSchemeTable.create }
      { Version = v "3.0.0"; Update = AccountTable.addVersioningSchemeForeignKey }
      { Version = v "3.0.1"; Update = VersioningSchemeTable.addSemVer2DefaultScheme }
      { Version = v "3.1.0"; Update = RoleTable.create }
      { Version = v "3.2.0"; Update = RolePermissionTable.create }
      { Version = v "3.3.0"; Update = ApiKeyTable.create }
      { Version = v "3.4.0"; Update = AccountUserTable.create }
      { Version = v "3.5.0"; Update = ProductTable.create }
      { Version = v "3.6.0"; Update = AccountTable.addChangeTrackerAccount }
      { Version = v "4.0.0"; Update = ProductTable.addUniqueIndexOnAccountIdAndName }
      { Version = v "4.1.0"; Update = ProductUserTable.create }
      { Version = v "4.2.0"; Update = VersionTable.create }
      { Version = v "5.0.0"; Update = VersionTable.addUniqueIndexProductIdValueDeletedAt }
      { Version = v "5.1.0"; Update = ChangeLogLineTable.create }
      { Version = v "6.0.0"; Update = ChangeLogLineTable.addPartialUniqueIndexOnProductIdVersionIdTextDeletedAt }
      { Version = v "6.1.0"; Update = Functions.createGuidFunction }
      { Version = v "6.2.0"; Update = RoleTable.addBasicRoles }
      { Version = v "7.0.0"; Update = VersioningSchemeTable.addUniqueIndexOnNameAccountIdDeletedAt }
      { Version = v "7.1.0"; Update = RolePermissionTable.addPermissionAddOrUpdateProduct }
      { Version = v "7.2.0"; Update = RolePermissionTable.addPermissionViewChangeLogLines }
      { Version = v "7.3.0"; Update = RolePermissionTable.addSomeViewPermissions }
      { Version = v "7.4.0"; Update = AccountViews.createAccountUserRolesView }
      { Version = v "7.5.0"; Update = ProductViews.createProductUserRolesView }
      { Version = v "8.0.0"; Update = UserTable.fixEmailUniqueConstraint }
      { Version = v "8.1.0"; Update = VersionTable.addTextSearch }
      { Version = v "8.2.0"; Update = AccountUserTable.createIndexOnUserIdAndRoleId }
      { Version = v "8.3.0"; Update = RolePermissionTable.addVersionPermissions }
      { Version = v "8.4.0"; Update = UserTable.addUserForAppChanges }
      { Version = v "8.5.0"; Update = ProductTable.addProductForAppChanges }
      { Version = v "8.6.0"; Update = RolePermissionTable.addChangeLogLinePermissions }
      { Version = v "8.7.0"; Update = LanguageTable.addLanguageTableWithBasicLanguages }
      { Version = v "8.8.0"; Update = ProductTable.addLanguageCodes }
      { Version = v "9.0.0"; Update = VersionTable.makeUpdateSearchVectorsProcedureLanguageAware }
      { Version = v "10.0.0"; Update = ChangeLogLineTable.addPartialUniqueIndexOnProductIdVersionIdPositionDeletedAt }
      { Version = v "11.0.0"; Update = VersionTable.dropUpdateAllVersionSearchVectorsProcedure }
      { Version = v "12.0.0"; Update = RolePermissionTable.deleteObsoletePermissions }
      { Version = v "12.1.0"; Update = ExternalIdentityTable.create }
      { Version = v "12.1.1"; Update = AccountTable.fixUniqueIndexOnAccountName }
      { Version = v "12.1.2"; Update = ChangeLogLineTable.fixUniqueIndices }
      { Version = v "12.1.3"; Update = VersioningSchemeTable.fixUniqueIndexOnNameAndAccountId }
      { Version = v "12.1.4"; Update = VersionTable.fixUniqueIndexOnProductIdAndValue }
      { Version = v "12.1.5"; Update = VersionTable.addProductIdToSearchVectorIndex } ]