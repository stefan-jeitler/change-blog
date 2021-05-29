namespace ChangeTracker.Application.UseCases
{
    public enum Permission
    {
        AddProduct,
        CloseProduct,
        ViewAccountProducts,
        ViewRoles,
        ViewAccount,
        ViewAccountUsers,
        ViewUserProducts,
        ViewOwnUser,

        AddVersion,
        ViewCompleteVersion,
        ReleaseVersion,
        DeleteVersion,
        UpdateVersion,

        ViewPendingChangeLogLines,
        ViewChangeLogLines,
    }
}