using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogs;

public class GetPendingChangeLogsInteractor : IGetPendingChangeLogs
{
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IProductDao _productDao;
    private readonly IUserDao _userDao;

    public GetPendingChangeLogsInteractor(IChangeLogQueriesDao changeLogQueries, IUserDao userDao,
        IProductDao productDao)
    {
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
    }

    public async Task<PendingChangeLogsResponseModel> ExecuteAsync(Guid userId, Guid productId)
    {
        Guard.Against.NullOrEmpty(userId, nameof(userId));
        Guard.Against.NullOrEmpty(productId, nameof(productId));

        return await GetChangeLogsAsync(userId, productId);
    }

    private async Task<PendingChangeLogsResponseModel> GetChangeLogsAsync(Guid userId, Guid productId)
    {
        var currentUser = await _userDao.GetUserAsync(userId);
        var product = await _productDao.GetProductAsync(productId);

        var pendingChangeLogs = await _changeLogQueries.GetChangeLogsAsync(productId);

        var lines = pendingChangeLogs
            .Lines
            .Select(x => new ChangeLogLineResponseModel(x.Id,
                x.Text,
                x.Labels.Select(l => l.Value).ToList(),
                x.Issues.Select(i => i.Value).ToList(),
                x.CreatedAt.ToLocal(currentUser.TimeZone)
            )).ToList();


        return new PendingChangeLogsResponseModel(product.Id,
            product.Name,
            product.AccountId,
            lines);
    }
}