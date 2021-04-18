using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.Labels.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;
// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Labels.AddChangeLogLineLabel
{
    public class AddChangeLogLineLabelInteractor : IAddChangeLogLineLabel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChangeLogDao _changeLogDao;
        private readonly IVersionDao _versionDao;

        public AddChangeLogLineLabelInteractor(IUnitOfWork unitOfWork, IChangeLogDao changeLogDao, IVersionDao versionDao)
        {
            _unitOfWork = unitOfWork;
            _changeLogDao = changeLogDao;
            _versionDao = versionDao;
        }

        public async Task ExecuteAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLineLabelRequestModel requestModel)
        {
            _unitOfWork.Start();

            var line = await _changeLogDao.FindLineAsync(requestModel.ChangeLogLineId);
            if (line.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
                return;
            }

            if (!Label.TryParse(requestModel.Label, out var label))
            {
                output.InvalidLabel(requestModel.Label);
                return;
            }

            if (line.Value.IsPending)
            {
                await AddLabelAsync(output, line.Value, label);
                return;
            }

            if (await IsVersionReadOnlyAsync(output, line.Value))
                return;

            await AddLabelAsync(output, line.Value, label);
        }

        private async Task<bool> IsVersionReadOnlyAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLine line)
        {
            var version = await _versionDao.GetVersionAsync(line.ProjectId, line.VersionId!.Value);

            if (version.IsReleased)
            {
                output.RelatedVersionAlreadyReleased();
                return true;
            }

            if (version.IsDeleted)
            {
                output.RelatedVersionDeleted();
                return true;
            }

            return false;
        }

        private async Task AddLabelAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLine line, Label label)
        {
            if (line.AvailableLabelPlaces <= 0)
            {
                output.MaxLabelsReached(ChangeLogLine.MaxLabels);
                return;
            }

            line.AddLabel(label);

            await _changeLogDao.UpdateLineAsync(line)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.Added(line.Id);
            }
        }
    }
}
