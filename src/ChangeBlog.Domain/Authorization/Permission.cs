namespace ChangeBlog.Domain.Authorization;

public enum Permission
{
    ViewAccount,
    ViewAccountUsers,
    UpdateAccount,
    DeleteAccount,

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