namespace ChangeBlog.Domain.Authorization;

public enum Permission
{
    ViewAccount,
    ViewAccountUsers,
    UpdateAccount,
    DeleteAccount,

    AddOrUpdateProduct,

    //ViewAccountProducts,
    //CloseProduct,
    ViewProduct,
    FreezeProduct,

    AddOrUpdateVersion,
    //AddVersion,

    ViewVersions,
    ReleaseVersion,
    DeleteVersion,

    ViewPendingChangeLogLines,
    ViewChangeLogLines,
    AddOrUpdateChangeLogLine,
    DeleteChangeLogLine,
    MoveChangeLogLines
}