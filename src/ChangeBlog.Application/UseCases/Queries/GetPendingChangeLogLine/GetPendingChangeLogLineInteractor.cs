using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Application.UseCases.Queries.SharedModels;

namespace ChangeBlog.Application.UseCases.Queries.GetPendingChangeLogLine;

public class GetPendingChangeLogLineInteractor : IGetPendingChangeLogLine
{
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IProductDao _productDao;
    private readonly IUserDao _userDao;

    public GetPendingChangeLogLineInteractor(IChangeLogQueriesDao changeLogQueries, IUserDao userDao,
        IProductDao productDao)
    {
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
    }

    public async Task ExecuteAsync(IGetPendingChangeLogLineOutputPort output,
        Guid userId, Guid changeLogLineId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.");
        }

        if (changeLogLineId == Guid.Empty)
        {
            throw new ArgumentException("ChangeLogLineId cannot be empty.");
        }

        await GetChangeLogLineAsync(output, userId, changeLogLineId);
    }

    private async Task GetChangeLogLineAsync(
        IGetPendingChangeLogLineOutputPort output,
        Guid userId,
        Guid changeLogLineId)
    {
        var line = await _changeLogQueries.FindLineAsync(changeLogLineId);

        if (line.HasNoValue)
        {
            output.LineDoesNotExist(changeLogLineId);
            return;
        }

        if (!line.GetValueOrThrow().IsPending)
        {
            output.LineIsNotPending(changeLogLineId);
            return;
        }

        var currentUser = await _userDao.GetUserAsync(userId);
        var product = await _productDao.GetProductAsync(line.GetValueOrThrow().ProductId);

        var l = line.GetValueOrThrow();
        var lineResponseModel = new ChangeLogLineResponseModel(
            l.Id,
            l.Text,
            l.Labels.Select(ll => ll.Value).ToList(),
            l.Issues.Select(i => i.Value).ToList(),
            l.CreatedAt.ToLocal(currentUser.TimeZone));

        output.LineFound(new PendingChangeLogLineResponseModel(product.Id,
            product.Name,
            product.AccountId,
            lineResponseModel));
    }
}