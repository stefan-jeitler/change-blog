using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Application.ChangeLogLineParser;
using ChangeBlog.Domain;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.ChangeLogs.AddPendingChangeLogLine;

public class AddPendingChangeLogLineInteractor : IAddPendingChangeLogLine
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IProductDao _productDao;
    private readonly IBusinessTransaction _businessTransaction;

    public AddPendingChangeLogLineInteractor(IProductDao productDao, IChangeLogQueriesDao changeLogQueriesDao,
        IChangeLogCommandsDao changeLogCommands, IBusinessTransaction businessTransaction)
    {
        _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
        _changeLogQueries = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
        _businessTransaction = businessTransaction ?? throw new ArgumentNullException(nameof(businessTransaction));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
    }

    public async Task ExecuteAsync(IAddPendingChangeLogLineOutputPort output,
        PendingChangeLogLineRequestModel lineRequestModel)
    {
        _businessTransaction.Start();

        var product = await _productDao.FindProductAsync(lineRequestModel.ProductId);
        if (product.HasNoValue)
        {
            output.ProductDoesNotExist(lineRequestModel.ProductId);
            return;
        }

        var line = await CreateChangeLogLineAsync(output, lineRequestModel, product.GetValueOrThrow());
        if (line.HasNoValue)
        {
            return;
        }

        await SaveChangeLogLineAsync(output, line.GetValueOrThrow());
    }

    private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddPendingChangeLogLineOutputPort output,
        PendingChangeLogLineRequestModel requestModel,
        Product product)
    {
        var parsedLine = ParseLine(output, requestModel);
        if (parsedLine.HasNoValue)
        {
            return Maybe<ChangeLogLine>.None;
        }

        var changeLogs = await _changeLogQueries.GetChangeLogsAsync(product.Id);

        if (!changeLogs.IsPositionAvailable)
        {
            output.TooManyLines(Domain.ChangeLog.ChangeLogs.MaxLines);
            return Maybe<ChangeLogLine>.None;
        }

        var existingLineWithSameText =
            changeLogs.Lines.FirstOrDefault(x => x.Text.Equals(parsedLine.GetValueOrThrow().Text));
        if (existingLineWithSameText is not null)
        {
            output.LinesWithSameTextsAreNotAllowed(existingLineWithSameText.Id, parsedLine.GetValueOrThrow().Text);
            return Maybe<ChangeLogLine>.None;
        }

        var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
            null,
            product.Id,
            parsedLine.GetValueOrThrow().Text,
            changeLogs.NextFreePosition,
            DateTime.UtcNow,
            parsedLine.GetValueOrThrow().Labels,
            parsedLine.GetValueOrThrow().Issues,
            requestModel.UserId);

        return Maybe<ChangeLogLine>.From(changeLogLine);
    }

    private static Maybe<LineParserResponseModel> ParseLine(ILineParserOutput output,
        PendingChangeLogLineRequestModel requestModel)
    {
        var lineParsingRequestModel =
            new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);

        return LineParser.Parse(output, lineParsingRequestModel);
    }

    private async Task SaveChangeLogLineAsync(IAddPendingChangeLogLineOutputPort outputPort, ChangeLogLine line)
    {
        await _changeLogCommands
            .AddOrUpdateLineAsync(line)
            .Match(Finish, outputPort.Conflict);

        void Finish(ChangeLogLine l)
        {
            _businessTransaction.Commit();
            outputPort.Created(l.Id);
        }
    }
}