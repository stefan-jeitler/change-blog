using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.Accounts;
using ChangeBlog.Application.DataAccess.Products;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Common;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeBlog.Application.UseCases.Commands.AddProduct
{
    public class AddProductInteractor : IAddProduct
    {
        private readonly IAccountDao _accountDao;
        private readonly IProductDao _productDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersioningSchemeDao _versioningSchemeDao;

        public AddProductInteractor(IAccountDao accountDao, IVersioningSchemeDao versioningSchemeDao,
            IProductDao productDao, IUnitOfWork unitOfWork)
        {
            _accountDao = accountDao ?? throw new ArgumentNullException(nameof(accountDao));
            _versioningSchemeDao = versioningSchemeDao ?? throw new ArgumentNullException(nameof(versioningSchemeDao));
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IAddProductOutputPort output, ProductRequestModel productRequestModel)
        {
            _unitOfWork.Start();

            var account = await GetAccountAsync(output, productRequestModel.AccountId);
            if (account.HasNoValue)
                return;

            if (!Name.TryParse(productRequestModel.Name, out var name))
            {
                output.InvalidName(productRequestModel.Name);
                return;
            }

            var existingProduct = await _productDao.FindProductAsync(account.Value.Id, name);
            if (existingProduct.HasValue)
            {
                output.ProductAlreadyExists(existingProduct.Value.Id);
                return;
            }

            var versioningSchemeId = await GetVersioningSchemeIdAsync(output, productRequestModel, account.Value);
            if (versioningSchemeId.HasNoValue)
                return;

            var langCode = await GetLangCodeAsync(output, productRequestModel.LanguageCode);
            if (langCode.HasNoValue)
                return;

            var product = new Product(account.Value.Id,
                name, versioningSchemeId.Value,
                productRequestModel.UserId,
                langCode.Value,
                DateTime.UtcNow);

            await SaveProductAsync(output, product);
        }

        private async Task<Maybe<Name>> GetLangCodeAsync(IAddProductOutputPort output, string languageCode)
        {
            var supportedLangCodes = await _productDao.GetSupportedLanguageCodesAsync();

            if (!Name.TryParse(languageCode, out var productLangCode))
            {
                output.NotSupportedLanguageCode(languageCode, supportedLangCodes.Select(x => x.Value).ToList());
                return Maybe<Name>.None;
            }

            var langCode = supportedLangCodes.TryFirst(x =>
                x.Value.Equals(productLangCode.Value, StringComparison.OrdinalIgnoreCase));

            if (langCode.HasNoValue)
            {
                output.NotSupportedLanguageCode(productLangCode.Value,
                    supportedLangCodes.Select(x => x.Value).ToList());
                return Maybe<Name>.None;
            }

            return langCode;
        }

        private async Task<Maybe<Account>> GetAccountAsync(IAddProductOutputPort output, Guid accountId)
        {
            var account = await _accountDao.FindAccountAsync(accountId);
            if (account.HasNoValue)
            {
                output.AccountDoesNotExist(accountId);
                return Maybe<Account>.None;
            }

            if (account.Value.DeletedAt.HasValue)
            {
                output.AccountDeleted(account.Value.Id);
                return Maybe<Account>.None;
            }

            return account;
        }

        private async Task<Maybe<VersioningScheme>> GetVersioningSchemeIdAsync(IAddProductOutputPort output,
            ProductRequestModel productRequestModel, Account account)
        {
            var versioningSchemeService = new VersioningSchemeIdFinder(account);
            var customSchemeId = productRequestModel.VersioningSchemeId;
            var versioningSchemeId = versioningSchemeService.FindSchemeIdForProduct(customSchemeId);

            var scheme = await _versioningSchemeDao.FindSchemeAsync(versioningSchemeId);

            if (scheme.HasNoValue)
            {
                output.VersioningSchemeDoesNotExist(versioningSchemeId);
                return Maybe<VersioningScheme>.None;
            }

            var isCommonScheme = !scheme.Value.AccountId.HasValue;
            if (isCommonScheme)
                return scheme;

            var schemeBelongsToAccount = scheme.Value.AccountId == account.Id;
            if (!schemeBelongsToAccount)
            {
                output.VersioningSchemeDoesNotExist(scheme.Value.Id);
                return Maybe<VersioningScheme>.None;
            }

            return scheme;
        }

        private async Task SaveProductAsync(IAddProductOutputPort output, Product newProduct)
        {
            await _productDao
                .AddProductAsync(newProduct)
                .Match(Finish, output.Conflict);

            void Finish(Product x)
            {
                _unitOfWork.Commit();
                output.Created(x.AccountId, x.Id);
            }
        }
    }
}
