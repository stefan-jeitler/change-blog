using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.ChangeLog;
using ChangeBlog.Application.DataAccess.Products;
using ChangeBlog.Application.Services.ChangeLogLineParsing;
using ChangeBlog.Domain;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Commands.AddPendingChangeLogLine
{
    public class AddPendingChangeLogLineInteractor : IAddPendingChangeLogLine
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IProductDao _productDao;
        private readonly IUnitOfWork _unitOfWork;

        public AddPendingChangeLogLineInteractor(IProductDao productDao, IChangeLogQueriesDao changeLogQueriesDao,
            IChangeLogCommandsDao changeLogCommands, IUnitOfWork unitOfWork)
        {
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _changeLogQueries = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IAddPendingChangeLogLineOutputPort output,
            PendingChangeLogLineRequestModel lineRequestModel)
        {
            _unitOfWork.Start();

            var product = await _productDao.FindProductAsync(lineRequestModel.ProductId);
            if (product.HasNoValue)
            {
                output.ProductDoesNotExist(lineRequestModel.ProductId);
                return;
            }

            var line = await CreateChangeLogLineAsync(output, lineRequestModel, product.Value);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddPendingChangeLogLineOutputPort output,
            PendingChangeLogLineRequestModel requestModel,
            Product product)
        {
            var parsedLine = ParseLine(output, requestModel);
            if (parsedLine.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogs = await _changeLogQueries.GetChangeLogsAsync(product.Id);

            if (!changeLogs.IsPositionAvailable)
            {
                output.TooManyLines(ChangeLogs.MaxLines);
                return Maybe<ChangeLogLine>.None;
            }

            var existingLineWithSameText = changeLogs.Lines.FirstOrDefault(x => x.Text.Equals(parsedLine.Value.Text));
            if (existingLineWithSameText is not null)
            {
                output.LinesWithSameTextsAreNotAllowed(existingLineWithSameText.Id, parsedLine.Value.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                null,
                product.Id,
                parsedLine.Value.Text,
                changeLogs.NextFreePosition,
                DateTime.UtcNow,
                parsedLine.Value.Labels,
                parsedLine.Value.Issues,
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
                _unitOfWork.Commit();
                outputPort.Created(l.Id);
            }
        }
    }
}
