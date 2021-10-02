using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.ChangeLog;
using ChangeBlog.Application.UseCases.Commands.Issues.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Commands.Issues.AddChangeLogLineIssue
{
    public class AddChangeLogLineIssueInteractor : IAddChangeLogLineIssue
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;

        public AddChangeLogLineIssueInteractor(IUnitOfWork unitOfWork, IChangeLogQueriesDao changeLogQueries,
            IChangeLogCommandsDao changeLogCommands)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IAddChangeLogLineIssueOutputPort output,
            ChangeLogLineIssueRequestModel requestModel)
        {
            if (!Issue.TryParse(requestModel.Issue, out var issue))
            {
                output.InvalidIssue(requestModel.Issue);
                return;
            }

            _unitOfWork.Start();

            var line = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);
            if (line.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
                return;
            }

            if (line.GetValueOrThrow().AvailableIssuePlaces <= 0)
            {
                output.MaxIssuesReached(ChangeLogLine.MaxIssues);
                return;
            }

            await AddIssueAsync(output, line.GetValueOrThrow(), issue);
        }

        private async Task AddIssueAsync(IAddChangeLogLineIssueOutputPort output, ChangeLogLine line, Issue issue)
        {
            line.AddIssue(issue);

            await _changeLogCommands.UpdateLineAsync(line)
                .Match(Finish, output.Conflict);

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Added(l.Id);
            }
        }
    }
}
