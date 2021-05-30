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
        AddCompleteVersion,
        ViewCompleteVersions,
        ReleaseVersion,
        DeleteVersion,

        ViewPendingChangeLogLines,
        ViewChangeLogLines,
        AddOrUpdateChangeLogLine,
        DeleteChangeLogLine,
        MoveChangeLogLines
    }
}