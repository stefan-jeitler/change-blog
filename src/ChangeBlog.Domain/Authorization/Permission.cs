namespace ChangeBlog.Domain.Authorization
{
    public enum Permission
    {
        ViewOwnUser,
        ViewAccount,
        ViewAccountUsers,
        ViewRoles,

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
