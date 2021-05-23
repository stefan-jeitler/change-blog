using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine
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

        public async Task ExecuteAsync(IAddPendingLineOutputPort output,
            PendingLineRequestModel lineRequestModel)
        {
            _unitOfWork.Start();

            var product = await _productDao.FindProductAsync(lineRequestModel.ProductId);
            if (product.HasNoValue)
            {
                output.ProductDoesNotExist();
                return;
            }

            var line = await CreateChangeLogLineAsync(output, lineRequestModel, product.Value);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddPendingLineOutputPort output,
            PendingLineRequestModel requestModel,
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

            if (changeLogs.ContainsText(parsedLine.Value.Text))
            {
                output.LineWithSameTextAlreadyExists(parsedLine.Value.Text);
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
            PendingLineRequestModel requestModel)
        {
            var lineParsingRequestModel =
                new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);

            return LineParser.Parse(output, lineParsingRequestModel);
        }

        private async Task SaveChangeLogLineAsync(IAddPendingLineOutputPort outputPort, ChangeLogLine line)
        {
            await _changeLogCommands
                .AddLineAsync(line)
                .Match(Finish, c => outputPort.Conflict(c));

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                outputPort.Created(l.Id);
            }
        }
    }
}