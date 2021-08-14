using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.Products;
using ChangeBlog.Application.DataAccess.Versions;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeBlog.Application.UseCases.Commands.ReleaseVersion
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
                    output.Conflict);
        }

        private async Task<Maybe<ClVersion>> GetVersionAsync(IReleaseVersionOutputPort output, Guid versionId)
        {
            var clVersion = await _versionDao.FindVersionAsync(versionId);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist(versionId);
                return Maybe<ClVersion>.None;
            }

            if (clVersion.Value.IsDeleted)
            {
                output.VersionAlreadyDeleted(clVersion.Value.Id);
                return Maybe<ClVersion>.None;
            }

            if (clVersion.Value.IsReleased)
            {
                output.VersionAlreadyReleased(clVersion.Value.Id);
                return Maybe<ClVersion>.None;
            }

            return clVersion;
        }
    }
}
