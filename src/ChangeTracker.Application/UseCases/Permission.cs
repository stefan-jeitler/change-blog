namespace ChangeTracker.Application.UseCases
{
    public enum Permission
    {
        ViewRoles,
        ViewAccount,
        ViewAccountUsers,
        ViewOwnUser,

        ViewUserProducts,
        ViewAccountProducts,
        CloseProduct,
        AddOrUpdateProduct,

        AddOrUpdateVersion,
        AddVersion,
        ViewVersions,
        ReleaseVersion,
        DeleteVersion,

        ViewPendingChangeLogLines,
        ViewChangeLogLines,
        AddOrUpdateChangeLogLine,
        DeleteChangeLogLine,
        MoveChangeLogLines
    }
}