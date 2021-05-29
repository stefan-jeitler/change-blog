using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.UpdateVersion
{
    public class UpdateVersionInteractor : IUpdateVersion
    {
        private readonly IProductDao _productDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public UpdateVersionInteractor(IVersionDao versionDao, IProductDao productDao, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IUpdateVersionOutputPort output, UpdateVersionRequestModel requestModel)
        {
            if (!ClVersionValue.TryParse(requestModel.Version, out var clVersionValue))
            {
                output.InvalidVersionFormat(requestModel.Version);
                return;
            }

            if (!OptionalName.TryParse(requestModel.Name, out var clVersionName))
            {
                output.InvalidVersionName(requestModel.Name);
                return;
            }

            _unitOfWork.Start();

            var clVersion = await GetVersionAsync(output, requestModel, clVersionValue);
            if (clVersion.HasNoValue)
                return;


            if (clVersionValue == clVersion.Value.Value &&
                clVersionName == clVersion.Value.Name)
            {
                output.VersionUpdated(clVersion.Value.Id);
                return;
            }

            var existingVersionWithSameValue =
                await _versionDao.FindVersionAsync(clVersion.Value.ProductId, clVersionValue);
            if (existingVersionWithSameValue.HasValue &&
                existingVersionWithSameValue.Value.Id != clVersion.Value.Id)
            {
                output.VersionWithSameValueAlreadyExists(clVersionValue);
                return;
            }

            await UpdateVersionAsync(output, clVersion, clVersionValue, clVersionName);
        }

        private async Task<Maybe<ClVersion>> GetVersionAsync(IUpdateVersionOutputPort output,
            UpdateVersionRequestModel requestModel, ClVersionValue clVersionValue)
        {
            var clVersion = await GetVersionAsync(output, requestModel.VersionId);
            if (clVersion.HasNoValue)
                return Maybe<ClVersion>.None;


            var product = await _productDao.GetProductAsync(clVersion.Value.ProductId);
            if (product.IsClosed)
            {
                output.RelatedProductClosed(product.Id);
                return Maybe<ClVersion>.None;
            }

            if (!clVersionValue.Match(product.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(clVersionValue);
                return Maybe<ClVersion>.None;
            }

            return clVersion;
        }

        private async Task UpdateVersionAsync(IUpdateVersionOutputPort output, Maybe<ClVersion> clVersion,
            ClVersionValue clVersionValue,
            OptionalName clVersionName)
        {
            var v = clVersion.Value;
            var updatedClVersion = new ClVersion(v.Id, v.ProductId, clVersionValue, clVersionName, v.ReleasedAt,
                v.CreatedByUser,
                v.CreatedAt, v.DeletedAt);

            await _versionDao.UpdateVersionAsync(updatedClVersion)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(ClVersion version)
            {
                _unitOfWork.Commit();
                output.VersionUpdated(version.Id);
            }
        }

        private async Task<Maybe<ClVersion>> GetVersionAsync(IUpdateVersionOutputPort output, Guid versionId)
        {
            var clVersion = await _versionDao.FindVersionAsync(versionId);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return Maybe<ClVersion>.None;
            }

            if (clVersion.Value.IsDeleted)
            {
                output.VersionAlreadyDeleted();
                return Maybe<ClVersion>.None;
            }

            if (clVersion.Value.IsReleased)
            {
                output.VersionAlreadyReleased();
                return Maybe<ClVersion>.None;
            }

            return clVersion;
        }
    }
}