using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Application.Boundaries.DataAccess.Versions;
using ChangeBlog.Application.ChangeLogLineParser;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.OutputPorts;
using ChangeBlog.Domain;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion;

public class AddOrUpdateVersionInteractor : IAddVersion, IAddOrUpdateVersion
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueriesDao;
    private readonly IProductDao _productDao;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVersionDao _versionDao;

    public AddOrUpdateVersionInteractor(IProductDao productDao, IVersionDao versionDao,
        IUnitOfWork unitOfWork, IChangeLogCommandsDao changeLogCommands, IChangeLogQueriesDao changeLogQueriesDao)
    {
        _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
        _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        _changeLogQueriesDao = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
    }

    public async Task ExecuteAsync(IAddOrUpdateVersionOutputPort output, VersionRequestModel requestModel)
    {
        if (!ClVersionValue.TryParse(requestModel.Version, out var clVersionValue))
        {
            output.InvalidVersionFormat(requestModel.Version);
            return;
        }

        _unitOfWork.Start();
        var clVersion = await _versionDao.FindVersionAsync(requestModel.ProductId, clVersionValue);

        if (clVersion.HasValue)
        {
            await UpdateExistingVersionAsync(output, clVersion.GetValueOrThrow(), clVersionValue, requestModel);
        }
        else
        {
            await ExecuteAsync((IAddVersionOutputPort)output, requestModel);
            _unitOfWork.Commit();
        }
    }

    public async Task ExecuteAsync(IAddVersionOutputPort output,
        VersionRequestModel versionRequestModel)
    {
        var product = await GetProductAsync(output, versionRequestModel.ProductId);
        if (product.HasNoValue)
        {
            return;
        }

        var newVersion = CreateNewVersion(output, product.GetValueOrThrow(), versionRequestModel);
        if (newVersion.HasNoValue)
        {
            return;
        }

        var existingVersion =
            await _versionDao.FindVersionAsync(product.GetValueOrThrow().Id, newVersion.GetValueOrThrow().Value);
        if (existingVersion.HasValue)
        {
            output.VersionAlreadyExists(existingVersion.GetValueOrThrow().Id);
            return;
        }

        var lines = newVersion.Bind(v => CreateLines(output, versionRequestModel.Lines, v));
        if (lines.HasNoValue)
        {
            return;
        }

        _unitOfWork.Start();
        await SaveVersionAsync(output, newVersion.GetValueOrThrow(), lines.GetValueOrThrow(),
            versionRequestModel.ReleaseImmediately);
    }

    private async Task<Maybe<Product>> GetProductAsync(IAddVersionOutputPort output, Guid productId)
    {
        var product = await _productDao.FindProductAsync(productId);
        if (product.HasNoValue)
        {
            output.ProductDoesNotExist(productId);
            return Maybe<Product>.None;
        }

        if (product.GetValueOrThrow().IsClosed)
        {
            output.RelatedProductClosed(productId);
            return Maybe<Product>.None;
        }

        return product;
    }

    private static Maybe<ClVersion> CreateNewVersion(IAddVersionOutputPort output,
        Product product, VersionRequestModel requestModel)
    {
        if (!ClVersionValue.TryParse(requestModel.Version, out var clVersion))
        {
            output.InvalidVersionFormat(requestModel.Version);
            return Maybe<ClVersion>.None;
        }

        if (!clVersion.Match(product.VersioningScheme))
        {
            output.VersionDoesNotMatchScheme(clVersion.Value, product.VersioningScheme.Name);
            return Maybe<ClVersion>.None;
        }

        if (!OptionalName.TryParse(requestModel.Name, out var versionName))
        {
            output.InvalidVersionName(requestModel.Name);
            return Maybe<ClVersion>.None;
        }

        var version = new ClVersion(Guid.NewGuid(),
            product.Id,
            clVersion,
            versionName,
            null,
            requestModel.UserId,
            DateTime.UtcNow,
            null);

        return Maybe<ClVersion>.From(version);
    }

    private static Maybe<IEnumerable<ChangeLogLine>> CreateLines(IAddVersionOutputPort output,
        IEnumerable<ChangeLogLineRequestModel> requestModel, ClVersion newVersion)
    {
        var lineCandidates = requestModel
            .ToList();

        var duplicates = lineCandidates
            .GroupBy(x => x.Text.ToLower(CultureInfo.InvariantCulture))
            .Where(x => x.Skip(1).Any())
            .Select(x => x.Key)
            .ToList();

        if (duplicates.Any())
        {
            output.LinesWithSameTextsAreNotAllowed(duplicates);
            return Maybe<IEnumerable<ChangeLogLine>>.None;
        }

        if (lineCandidates.Count > Domain.ChangeLog.ChangeLogs.MaxLines)
        {
            output.TooManyLines(Domain.ChangeLog.ChangeLogs.MaxLines);
            return Maybe<IEnumerable<ChangeLogLine>>.None;
        }

        return ParseLines(output, lineCandidates, newVersion);
    }

    private static Maybe<IEnumerable<ChangeLogLine>> ParseLines(ILineParserOutput output,
        IEnumerable<ChangeLogLineRequestModel> lineCandidates, ClVersion newVersion)
    {
        var lines = new List<ChangeLogLine>();
        foreach (var (lineRequestModel, i) in lineCandidates.Select((x, i) => (x, i)))
        {
            var line = CreateLine(output, lineRequestModel, newVersion, (uint)i);

            if (line.HasNoValue)
            {
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            lines.Add(line.GetValueOrThrow());
        }

        return Maybe<IEnumerable<ChangeLogLine>>.From(lines);
    }

    private static Maybe<ChangeLogLine> CreateLine(ILineParserOutput output,
        ChangeLogLineRequestModel requestModel, ClVersion clVersion, uint position)
    {
        var lineParsingRequestModel =
            new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);

        var parsedLine = LineParser.Parse(output, lineParsingRequestModel);
        if (parsedLine.HasNoValue)
        {
            return Maybe<ChangeLogLine>.None;
        }

        var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
            clVersion.Id,
            clVersion.ProductId,
            parsedLine.GetValueOrThrow().Text,
            position,
            DateTime.UtcNow,
            parsedLine.GetValueOrThrow().Labels,
            parsedLine.GetValueOrThrow().Issues,
            clVersion.CreatedByUser);

        return Maybe<ChangeLogLine>.From(changeLogLine);
    }

    private async Task SaveVersionAsync(IAddVersionOutputPort output, ClVersion newVersion,
        IEnumerable<ChangeLogLine> lines, bool releaseImmediately)
    {
        await _versionDao.AddVersionAsync(newVersion)
            .Bind(_ => _changeLogCommands.AddOrUpdateLinesAsync(lines))
            .Bind(_ => releaseImmediately
                ? _versionDao.ReleaseVersionAsync(newVersion.Release())
                : Task.FromResult(Result.Success<ClVersion, Conflict>(newVersion)))
            .Match(Finish, output.InsertConflict);

        void Finish(ClVersion version)
        {
            _unitOfWork.Commit();
            output.Created(version.Id);
        }
    }

    private async Task UpdateExistingVersionAsync(IAddOrUpdateVersionOutputPort output,
        ClVersion clVersion, ClVersionValue newVersionValue, VersionRequestModel requestModel)
    {
        if (clVersion.IsDeleted)
        {
            output.VersionAlreadyDeleted(clVersion.Id);
            return;
        }

        if (clVersion.IsReleased)
        {
            output.VersionAlreadyReleased(clVersion.Id);
            return;
        }

        var product = await _productDao.GetProductAsync(clVersion.ProductId);
        if (product.IsClosed)
        {
            output.RelatedProductClosed(product.Id);
            return;
        }

        if (!newVersionValue.Match(product.VersioningScheme))
        {
            output.VersionDoesNotMatchScheme(clVersion.Value, product.VersioningScheme.Name);
            return;
        }

        await UpdateVersionAsync(output, clVersion, newVersionValue, requestModel);
    }

    private async Task UpdateVersionAsync(IAddOrUpdateVersionOutputPort output, ClVersion clVersion,
        ClVersionValue newVersionValue,
        VersionRequestModel requestModel)
    {
        var parsedChangeLogs = CreateLines(output, requestModel.Lines, clVersion)
            .Map(x => new Domain.ChangeLog.ChangeLogs(x.ToList()));
        if (parsedChangeLogs.HasNoValue)
        {
            return;
        }

        if (!OptionalName.TryParse(requestModel.Name, out var clVersionName))
        {
            output.InvalidVersionName(requestModel.Name);
            return;
        }

        var releasedAt = requestModel.ReleaseImmediately
            ? DateTime.UtcNow
            : (DateTime?)null;

        var v = clVersion;
        var updatedVersion = new ClVersion(v.Id, v.ProductId, newVersionValue, clVersionName, releasedAt,
            v.CreatedByUser, v.CreatedAt, v.DeletedAt);

        var existingChangeLogs = await _changeLogQueriesDao.GetChangeLogsAsync(clVersion.ProductId, clVersion.Id);
        var linesToDelete =
            existingChangeLogs.Lines.Where(x => !parsedChangeLogs.GetValueOrThrow().ContainsText(x.Text));
        await DeleteExistingLines(linesToDelete);

        await _changeLogCommands.AddOrUpdateLinesAsync(parsedChangeLogs.GetValueOrThrow().Lines)
            .Bind(_ => _versionDao.UpdateVersionAsync(updatedVersion))
            .Match(Finish, output.UpdateConflict);

        void Finish(ClVersion version)
        {
            _unitOfWork.Commit();
            output.VersionUpdated(v.Id);
        }
    }

    private async Task DeleteExistingLines(IEnumerable<ChangeLogLine> changeLogLines)
    {
        foreach (var line in changeLogLines)
        {
            await _changeLogCommands.DeleteLineAsync(line);
        }
    }
}