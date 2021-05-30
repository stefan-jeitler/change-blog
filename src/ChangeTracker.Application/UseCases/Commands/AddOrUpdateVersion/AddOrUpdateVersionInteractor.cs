using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.Commands.AddVersion;
using ChangeTracker.Application.UseCases.Commands.SharedModels;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion
{
    public class AddOrUpdateVersionInteractor : IAddOrUpdateVersion
    {
        private readonly IAddVersion _addVersion;
        private readonly IProductDao _productDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddOrUpdateVersionInteractor(IVersionDao versionDao, IProductDao productDao, IUnitOfWork unitOfWork,
            IAddVersion addVersion)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _addVersion = addVersion ?? throw new ArgumentNullException(nameof(addVersion));
        }

        public async Task ExecuteAsync(IAddOrUpdateVersionOutputPort output, VersionRequestModel requestModel)
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
            var clVersion = await _versionDao.FindVersionAsync(requestModel.ProductId, clVersionValue);

            if (clVersion.HasValue)
            {
                await UpdateExistingVersionAsync(output, clVersion.Value, clVersionValue, clVersionName,
                    requestModel.ReleaseImmediately);
            }
            else
            {
                await _addVersion.ExecuteAsync(output, requestModel);
                _unitOfWork.Commit();
            }
        }

        private async Task UpdateExistingVersionAsync(IAddOrUpdateVersionOutputPort output,
            ClVersion clVersion, ClVersionValue clVersionValue, OptionalName clVersionName,
            bool releaseImmediately)
        {
            if (clVersion.IsDeleted)
            {
                output.VersionAlreadyDeleted();
                return;
            }

            if (clVersion.IsReleased)
            {
                output.VersionAlreadyReleased();
                return;
            }

            var product = await _productDao.GetProductAsync(clVersion.ProductId);
            if (product.IsClosed)
            {
                output.RelatedProductClosed(product.Id);
                return;
            }

            var existingVersionWithSameValue = await _versionDao.FindVersionAsync(product.Id, clVersionValue);
            if (existingVersionWithSameValue.HasValue &&
                existingVersionWithSameValue.Value.Id != clVersion.Id)
            {
                output.VersionWithSameValueAlreadyExists(clVersionValue);
                return;
            }

            if (!clVersion.Value.Match(product.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(clVersion.Value);
                return;
            }

            await UpdateVersionAsync(output, clVersion, clVersionValue, clVersionName, releaseImmediately);
        }

        private async Task UpdateVersionAsync(IAddOrUpdateVersionOutputPort output, ClVersion clVersion,
            ClVersionValue clVersionValue, OptionalName clVersionName, bool releaseImmediately)
        {
            var releasedAt = releaseImmediately
                ? DateTime.UtcNow
                : (DateTime?) null;

            var v = clVersion;
            var updatedVersion = new ClVersion(v.Id, v.ProductId, clVersionValue, clVersionName, releasedAt,
                v.CreatedByUser, v.CreatedAt, v.DeletedAt);

            await _versionDao.UpdateVersionAsync(updatedVersion)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(ClVersion version)
            {
                _unitOfWork.Commit();
                output.VersionUpdated(version.Id);
            }
        }
    }
}