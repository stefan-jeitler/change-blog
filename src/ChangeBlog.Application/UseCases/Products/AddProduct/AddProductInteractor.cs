using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeBlog.Application.UseCases.Products.AddProduct;

public class AddProductInteractor : IAddProduct
{
    private readonly IAccountDao _accountDao;
    private readonly IProductDao _productDao;
    private readonly IBusinessTransaction _businessTransaction;
    private readonly IVersioningSchemeDao _versioningSchemeDao;

    public AddProductInteractor(IAccountDao accountDao, IVersioningSchemeDao versioningSchemeDao,
        IProductDao productDao, IBusinessTransaction businessTransaction)
    {
        _accountDao = accountDao ?? throw new ArgumentNullException(nameof(accountDao));
        _versioningSchemeDao = versioningSchemeDao ?? throw new ArgumentNullException(nameof(versioningSchemeDao));
        _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
        _businessTransaction = businessTransaction ?? throw new ArgumentNullException(nameof(businessTransaction));
    }

    public async Task ExecuteAsync(IAddProductOutputPort output, ProductRequestModel productRequestModel)
    {
        _businessTransaction.Start();

        var account = await GetAccountAsync(output, productRequestModel.AccountId);
        if (account.HasNoValue)
            return;

        if (!Name.TryParse(productRequestModel.Name, out var name))
        {
            output.InvalidName(productRequestModel.Name);
            return;
        }

        var existingProduct = await _productDao.FindProductAsync(account.GetValueOrThrow().Id, name);
        if (existingProduct.HasValue)
        {
            output.ProductAlreadyExists(existingProduct.GetValueOrThrow().Id);
            return;
        }

        var versioningSchemeId =
            await GetVersioningSchemeIdAsync(output, productRequestModel, account.GetValueOrThrow());
        if (versioningSchemeId.HasNoValue)
        {
            return;
        }

        var langCode = await GetLangCodeAsync(output, productRequestModel.LanguageCode);
        if (langCode.HasNoValue)
        {
            return;
        }

        var product = new Product(account.GetValueOrThrow().Id,
            name, versioningSchemeId.GetValueOrThrow(),
            productRequestModel.UserId,
            langCode.GetValueOrThrow(),
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

        if (account.GetValueOrThrow().DeletedAt.HasValue)
        {
            output.AccountDeleted(account.GetValueOrThrow().Id);
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

        var isCommonScheme = !scheme.GetValueOrThrow().AccountId.HasValue;
        if (isCommonScheme)
        {
            return scheme;
        }

        var schemeBelongsToAccount = scheme.GetValueOrThrow().AccountId == account.Id;
        if (!schemeBelongsToAccount)
        {
            output.VersioningSchemeDoesNotExist(scheme.GetValueOrThrow().Id);
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
            _businessTransaction.Commit();
            output.Created(x.AccountId, x.Id);
        }
    }
}