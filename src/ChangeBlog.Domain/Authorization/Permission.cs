namespace ChangeBlog.Domain.Authorization;

public enum Permission
{
    ViewAccount,
    ViewAccountUsers,
    UpdateAccount,
    DeleteAccount,

    AddOrUpdateProduct,
    ViewProduct,
    FreezeProduct,

    AddOrUpdateVersion,
    ViewVersions,
    ReleaseVersion,
    DeleteVersion,

    ViewPendingChangeLogLines,
    ViewChangeLogLines,
    AddOrUpdateChangeLogLine,
    DeleteChangeLogLine,
    MoveChangeLogLines
}