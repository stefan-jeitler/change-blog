using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Application.Boundaries.DataAccess.Versions;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeBlog.Application.UseCases.Commands.DeleteVersion;

public class DeleteVersionInteractor : IDeleteVersion
{
    private readonly IProductDao _productDao;
    private readonly IVersionDao _versionDao;

    public DeleteVersionInteractor(IVersionDao versionDao, IProductDao productDao)
    {
        _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
        _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
    }

    public async Task ExecuteAsync(IDeleteVersionOutputPort output, Guid versionId)
    {
        var clVersion = await GetVersionAsync(output, versionId);
        if (clVersion.HasNoValue)
            return;

        var product = await _productDao.GetProductAsync(clVersion.GetValueOrThrow().ProductId);
        if (product.IsClosed)
        {
            output.RelatedProductClosed(product.Id);
            return;
        }

        var deletedVersion = clVersion.GetValueOrThrow().Delete();
        await _versionDao.DeleteVersionAsync(deletedVersion)
            .Match(
                v => output.VersionDeleted(v.Id),
                output.Conflict);
    }

    private async Task<Maybe<ClVersion>> GetVersionAsync(IDeleteVersionOutputPort output, Guid versionId)
    {
        var clVersion = await _versionDao.FindVersionAsync(versionId);
        if (clVersion.HasNoValue)
        {
            output.VersionDoesNotExist(versionId);
            return Maybe<ClVersion>.None;
        }

        var version = clVersion.GetValueOrThrow();

        if (version.IsDeleted)
        {
            output.VersionAlreadyDeleted(version.Id);
            return Maybe<ClVersion>.None;
        }

        if (version.IsReleased)
        {
            output.VersionAlreadyReleased(version.Id);
            return Maybe<ClVersion>.None;
        }

        return clVersion;
    }
}