using System;

namespace ChangeBlog.Application.Boundaries.DataAccess.Accounts;

public record AccountStats(Guid AccountId, int UsersCount, int ProductsCount);