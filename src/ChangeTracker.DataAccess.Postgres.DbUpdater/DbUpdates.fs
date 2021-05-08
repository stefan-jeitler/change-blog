namespace DbUpdater

module DbUpdates =
  open System.Data

  type DbUpdate = {
    Version: int
    Update: IDbConnection -> Async<unit>
  }

  let dbUpdates: DbUpdate list = [
    {Version = 0; Update = SchemaTable.create}
    {Version = 1; Update = AccountTable.create}
    {Version = 2; Update = VersioningSchemeTable.create}
    {Version = 3; Update = AccountTable.addVersioningSchemeForeignKey}
  ]