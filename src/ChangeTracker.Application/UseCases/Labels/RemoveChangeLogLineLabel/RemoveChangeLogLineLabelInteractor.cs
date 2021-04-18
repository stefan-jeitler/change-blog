using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.Labels.SharedModels;
using ChangeTracker.Domain.ChangeLog;

namespace ChangeTracker.Application.UseCases.Labels.RemoveChangeLogLineLabel
{
    public class RemoveChangeLogLineLabelInteractor : IRemoveChangeLogLineLabel
    {
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveChangeLogLineLabelInteractor(IUnitOfWork unitOfWork, IChangeLogQueriesDao changeLogQueries)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        }

        public async Task ExecuteAsync(IRemoveChangeLogLineLabelOutputPort output,
            ChangeLogLineLabelRequestModel requestModel)
        {
            if (!Label.TryParse(requestModel.Label, out var label))
            {
                output.InvalidLabel(requestModel.Label);
                return;
            }

            var line = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);
            if (line.HasNoValue)
            {
            }
        }
    }
}