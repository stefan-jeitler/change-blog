namespace ChangeBlog.Application.UseCases.Accounts.GetAccounts;

public class AccountStatsResponseModel
{
    public AccountStatsResponseModel(uint usersCount, uint productsCount)
    {
        UsersCount = usersCount;
        ProductsCount = productsCount;
    }

    public uint UsersCount { get; }
    public uint ProductsCount { get; }
}