module DbUpdates

open System.Data
open Semver

type DbUpdate =
    { Version: SemVersion
      Update: IDbConnection -> unit }

let dbUpdates: DbUpdate list =
    [ { Version = SemVersion.Parse("1.0.0"); Update = SchemaTable.create }
      { Version = SemVersion.Parse("1.1.0"); Update = AccountTable.create }
      { Version = SemVersion.Parse("2.0.0"); Update = AccountTable.addPartialUniqueIndexOnNameAndDeletedAt }
      { Version = SemVersion.Parse("2.1.0"); Update = UserTable.create }
      { Version = SemVersion.Parse("2.2.0"); Update = UserTable.addUserForDefaultVersioningSchemes }
      { Version = SemVersion.Parse("2.3.0"); Update = VersioningSchemeTable.create }
      { Version = SemVersion.Parse("3.0.0"); Update = AccountTable.addVersioningSchemeForeignKey }
      { Version = SemVersion.Parse("3.0.1"); Update = VersioningSchemeTable.addSemVer2DefaultScheme }
      { Version = SemVersion.Parse("3.1.0"); Update = RoleTable.create }
      { Version = SemVersion.Parse("3.2.0"); Update = RolePermissionTable.create }
      { Version = SemVersion.Parse("3.3.0"); Update = ApiKeyTable.create }
      { Version = SemVersion.Parse("3.4.0"); Update = AccountUserTable.create }
      { Version = SemVersion.Parse("3.5.0"); Update = ProductTable.create }
      { Version = SemVersion.Parse("3.6.0"); Update = AccountTable.addChangeTrackerAccount }
      { Version = SemVersion.Parse("4.0.0"); Update = ProductTable.addUniqueIndexOnAccountIdAndName }
      { Version = SemVersion.Parse("4.1.0"); Update = ProductUserTable.create }
      { Version = SemVersion.Parse("4.2.0"); Update = VersionTable.create }
      { Version = SemVersion.Parse("5.0.0"); Update = VersionTable.addUniqueIndexProductIdValueDeletedAt }
      { Version = SemVersion.Parse("5.1.0"); Update = ChangeLogLineTable.create }
      { Version = SemVersion.Parse("6.0.0"); Update = ChangeLogLineTable.addPartialUniqueIndexOnProductIdVersionIdTextDeletedAt }
      { Version = SemVersion.Parse("6.1.0"); Update = Functions.createGuidFunction }
      { Version = SemVersion.Parse("6.2.0"); Update = RoleTable.addBasicRoles }
      { Version = SemVersion.Parse("7.0.0"); Update = VersioningSchemeTable.addUniqueIndexOnNameAccountIdDeletedAt }
      { Version = SemVersion.Parse("7.1.0"); Update = RolePermissionTable.addPermissionAddOrUpdateProduct }
      { Version = SemVersion.Parse("7.2.0"); Update = RolePermissionTable.addPermissionViewChangeLogLines }
      { Version = SemVersion.Parse("7.3.0"); Update = RolePermissionTable.addSomeViewPermissions }
      { Version = SemVersion.Parse("7.4.0"); Update = AccountViews.createAccountUserRolesView }
      { Version = SemVersion.Parse("7.5.0"); Update = ProductViews.createProductUserRolesView }
      { Version = SemVersion.Parse("8.0.0"); Update = UserTable.fixEmailUniqueConstraint }
      { Version = SemVersion.Parse("8.1.0"); Update = VersionTable.addTextSearch }
      { Version = SemVersion.Parse("8.2.0"); Update = AccountUserTable.createIndexOnUserIdAndRoleId }
      { Version = SemVersion.Parse("8.3.0"); Update = RolePermissionTable.addVersionPermissions }
      { Version = SemVersion.Parse("8.4.0"); Update = UserTable.addUserForAppChanges }
      { Version = SemVersion.Parse("8.5.0"); Update = ProductTable.addProductForAppChanges }
      { Version = SemVersion.Parse("8.6.0"); Update = RolePermissionTable.addChangeLogLinePermissions }
      { Version = SemVersion.Parse("8.7.0"); Update = LanguageTable.addLanguageTableWithBasicLanguages }
      { Version = SemVersion.Parse("8.8.0"); Update = ProductTable.addLanguageCodes }
      { Version = SemVersion.Parse("9.0.0"); Update = VersionTable.makeUpdateSearchVectorsProcedureLanguageAware }
      { Version = SemVersion.Parse("10.0.0"); Update = ChangeLogLineTable.addPartialUniqueIndexOnProductIdVersionIdPositionDeletedAt }
      { Version = SemVersion.Parse("11.0.0"); Update = VersionTable.dropUpdateAllVersionSearchVectorsProcedure }
      { Version = SemVersion.Parse("12.0.0"); Update = RolePermissionTable.deleteObsoletePermissions }
      { Version = SemVersion.Parse("12.1.0"); Update = ExternalIdentityTable.create }
      { Version = SemVersion.Parse("12.1.1"); Update = AccountTable.fixUniqueIndexOnAccountName }
      { Version = SemVersion.Parse("12.1.2"); Update = ChangeLogLineTable.fixUniqueIndices }
      { Version = SemVersion.Parse("12.1.3"); Update = VersioningSchemeTable.fixUniqueIndexOnNameAndAccountId }
      { Version = SemVersion.Parse("12.1.4"); Update = VersionTable.fixUniqueIndexOnProductIdAndValue }
      { Version = SemVersion.Parse("12.1.5"); Update = VersionTable.addProductIdToSearchVectorIndex } ]
