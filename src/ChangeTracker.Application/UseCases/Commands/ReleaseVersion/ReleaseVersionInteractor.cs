using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.ReleaseVersion
{
    public class ReleaseVersionInteractor : IReleaseVersion
    {
        private readonly IProductDao _product;
        private readonly IVersionDao _versionDao;

        public ReleaseVersionInteractor(IVersionDao versionDao, IProductDao product)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _product = product ?? throw new ArgumentNullException(nameof(product));
        }

        public async Task ExecuteAsync(IReleaseVersionOutputPort output, Guid versionId)
        {
            var clVersion = await GetVersionAsync(output, versionId);
            if (clVersion.HasNoValue)
                return;

            var product = await _product.GetProductAsync(clVersion.Value.ProductId);
            if (product.IsClosed)
            {
                output.RelatedProductClosed(product.Id);
                return;
            }

            var releaseVersion = clVersion.Value.Release();
            await _versionDao.ReleaseVersionAsync(releaseVersion)
                .Match(
                    v => output.VersionReleased(v.Id),
                    c => output.Conflict(c.Reason));
        }

        private async Task<Maybe<ClVersion>> GetVersionAsync(IReleaseVersionOutputPort output, Guid versionId)
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