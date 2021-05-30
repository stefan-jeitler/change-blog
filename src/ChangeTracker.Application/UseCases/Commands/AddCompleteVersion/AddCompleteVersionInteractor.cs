using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion.Models;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeTracker.Application.UseCases.Commands.AddCompleteVersion
{
    public class AddCompleteVersionInteractor : IAddCompleteVersion
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IProductDao _productDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddCompleteVersionInteractor(IProductDao productDao, IVersionDao versionDao,
            IUnitOfWork unitOfWork, IChangeLogCommandsDao changeLogCommands)
        {
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IAddCompleteVersionOutputPort output,
            CompleteVersionRequestModel versionRequestModel)
        {
            var product = await GetProductAsync(output, versionRequestModel.ProductId);
            if (product.HasNoValue)
                return;

            var newVersion = CreateNewVersion(output, product.Value, versionRequestModel);
            if (newVersion.HasNoValue)
                return;

            if ((await _versionDao.FindVersionAsync(product.Value.Id, newVersion.Value.Value)).HasValue)
            {
                output.VersionAlreadyExists(newVersion.Value.Value);
                return;
            }

            var lines = newVersion.Bind(v => CreateLines(output, versionRequestModel.Lines, v));
            if (lines.HasNoValue)
                return;

            _unitOfWork.Start();
            await SaveCompleteVersionAsync(output, newVersion.Value, lines.Value);
        }

        private async Task<Maybe<Product>> GetProductAsync(IAddCompleteVersionOutputPort output, Guid productId)
        {
            var product = await _productDao.FindProductAsync(productId);
            if (product.HasNoValue)
            {
                output.ProductDoesNotExist(productId);
                return Maybe<Product>.None;
            }

            if (product.Value.IsClosed)
            {
                output.ProductClosed(productId);
                return Maybe<Product>.None;
            }

            return product;
        }

        private static Maybe<ClVersion> CreateNewVersion(IAddCompleteVersionOutputPort output,
            Product product, CompleteVersionRequestModel requestModel)
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

            var utcNow = DateTime.UtcNow;
            DateTime? releaseDate = requestModel.ReleaseImmediately ? utcNow : null;
            var version = new ClVersion(Guid.NewGuid(),
                product.Id,
                clVersion,
                versionName,
                releaseDate,
                requestModel.UserId,
                utcNow,
                null);

            return Maybe<ClVersion>.From(version);
        }

        private static Maybe<IEnumerable<ChangeLogLine>> CreateLines(IAddCompleteVersionOutputPort output,
            IEnumerable<ChangeLogLineRequestModel> requestModel, ClVersion newVersion)
        {
            var lineCandidates = requestModel
                .ToList();

            var duplicates = lineCandidates
                .GroupBy(x => x.Text)
                .Where(x => x.Skip(1).Any())
                .Select(x => x.Key)
                .ToList();

            if (duplicates.Any())
            {
                output.LinesWithSameTextsAreNotAllowed(duplicates);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            if (lineCandidates.Count > ChangeLogs.MaxLines)
            {
                output.TooManyLines(ChangeLogs.MaxLines);
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
                var line = CreateLine(output, lineRequestModel, newVersion, (uint) i);

                if (line.HasNoValue) return Maybe<IEnumerable<ChangeLogLine>>.None;

                lines.Add(line.Value);
            }

            return Maybe<IEnumerable<ChangeLogLine>>.From(lines);
        }

        private static Maybe<ChangeLogLine> CreateLine(ILineParserOutput output,
            ChangeLogLineRequestModel requestModel, ClVersion clVersion, uint position)
        {
            var lineParsingRequestModel =
                new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);

            var parsedLine = LineParser.Parse(output, lineParsingRequestModel);
            if (parsedLine.HasNoValue) return Maybe<ChangeLogLine>.None;

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                clVersion.Id,
                clVersion.ProductId,
                parsedLine.Value.Text,
                position,
                DateTime.UtcNow,
                parsedLine.Value.Labels,
                parsedLine.Value.Issues,
                clVersion.CreatedByUser);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task SaveCompleteVersionAsync(IAddCompleteVersionOutputPort output, ClVersion newVersion,
            IEnumerable<ChangeLogLine> lines)
        {
            await _versionDao
                .AddVersionAsync(newVersion)
                .Bind(_ => _changeLogCommands.AddLinesAsync(lines))
                .Match(Finish, c => output.Conflict(c));

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.Created(newVersion.Id);
            }
        }
    }
}