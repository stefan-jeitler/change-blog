﻿using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.AddVersion
{
    public class AddVersionInteractor : IAddVersion
    {
        private readonly IProductDao _productDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddVersionInteractor(IVersionDao versionDao, IProductDao productDao, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel versionRequestModel)
        {
            var (productId, v) = versionRequestModel;
            if (!ClVersionValue.TryParse(v, out var versionValue))
            {
                output.InvalidVersionFormat(v);
                return;
            }

            _unitOfWork.Start();
            var product = await GetProductAsync(output, productId);
            if (product.HasNoValue)
                return;

            var clVersion = await CreateVersionAsync(output, product.Value, versionValue);
            if (clVersion.HasNoValue)
                return;

            await SaveVersionAsync(output, clVersion.Value);
        }

        public async Task<Maybe<ClVersion>> CreateVersionAsync(IAddVersionOutputPort output, Product product,
            ClVersionValue versionValue)
        {
            var existingClVersion = await _versionDao.FindVersionAsync(product.Id, versionValue);
            if (existingClVersion.HasValue)
            {
                output.VersionAlreadyExists(versionValue);
                return Maybe<ClVersion>.None;
            }

            if (!versionValue.Match(product.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(versionValue);
                return Maybe<ClVersion>.None;
            }

            var clVersion = new ClVersion(Guid.NewGuid(),
                product.Id, versionValue,
                null, DateTime.UtcNow, null);

            return Maybe<ClVersion>.From(clVersion);
        }

        private async Task<Maybe<Product>> GetProductAsync(IAddVersionOutputPort output, Guid productId)
        {
            var product = await _productDao.FindProductAsync(productId);
            if (product.HasNoValue)
            {
                output.ProductDoesNotExist();
                return Maybe<Product>.None;
            }

            if (product.Value.IsClosed)
            {
                output.ProductClosed();
                return Maybe<Product>.None;
            }

            return product;
        }

        private async Task SaveVersionAsync(IAddVersionOutputPort output, ClVersion clVersion)
        {
            await _versionDao
                .AddVersionAsync(clVersion)
                .Match(Finish, c => output.Conflict(c));

            void Finish(ClVersion v)
            {
                _unitOfWork.Commit();
                output.Created(v.Id);
            }
        }
    }
}