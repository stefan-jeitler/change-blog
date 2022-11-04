using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Products.GetProducts;

public class GetProductsInteractor : IGetAccountProducts, IGetUserProducts, IGetProduct
{
    private readonly IAccountDao _accountDao;
    private readonly IProductDao _productDao;
    private readonly IUserDao _userDao;

    public GetProductsInteractor(IProductDao productDao, IUserDao userDao, IAccountDao accountDao)
    {
        _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        _accountDao = accountDao ?? throw new ArgumentNullException(nameof(accountDao));
    }

    public async Task<IList<ProductResponseModel>> ExecuteAsync(
        AccountProductQueryRequestModel requestModel)
    {
        var productQuerySettings = new AccountProductsQuerySettings(requestModel.AccountId,
            requestModel.UserId,
            requestModel.LastProductId,
            requestModel.Limit,
            requestModel.IncludeFreezedProducts);

        var products = await _productDao.GetAccountProductsAsync(productQuerySettings);
        var currentUser = await _userDao.GetUserAsync(requestModel.UserId);
        var creatorById = await GetProductCreatorsAsync(products);
        var accountById = await GetAccountsAsync(products);

        return products
            .Select(x => CreateResponse(x, accountById, creatorById, currentUser.TimeZone))
            .ToList();
    }

    public async Task<Maybe<ProductResponseModel>> ExecuteAsync(Guid userId, Guid productId)
    {
        var product = await _productDao.FindProductAsync(productId);
        var currentUser = await _userDao.GetUserAsync(userId);

        return await product
            .Map(async p => new
            {
                User = await _userDao.GetUserAsync(p.CreatedByUser),
                Account = await _accountDao.GetAccountAsync(p.AccountId),
                Product = p
            })
            .Map(x => CreateResponse(x.Product, x.User, x.Account.Name, currentUser.TimeZone));
    }

    public async Task<IList<ProductResponseModel>> ExecuteAsync(UserProductQueryRequestModel requestModel)
    {
        var querySettings = new UserProductsQuerySettings(requestModel.UserId,
            requestModel.LastProductId,
            requestModel.Limit,
            requestModel.IncludeFreezedProducts);

        var products = await _productDao.GetUserProductsAsync(querySettings);
        var currentUser = await _userDao.GetUserAsync(requestModel.UserId);
        var creatorById = await GetProductCreatorsAsync(products);
        var accountById = await GetAccountsAsync(products);

        return products
            .Select(x => CreateResponse(x, accountById, creatorById, currentUser.TimeZone))
            .ToList();
    }

    private async Task<IReadOnlyDictionary<Guid, Account>> GetAccountsAsync(IEnumerable<Product> products)
    {
        var accountIds = products
            .Select(x => x.AccountId)
            .Distinct()
            .ToList();

        var accounts = await _accountDao.GetAccountsAsync(accountIds);

        return accounts.ToDictionary(x => x.Id, x => x);
    }

    private async Task<IReadOnlyDictionary<Guid, User>> GetProductCreatorsAsync(IEnumerable<Product> products)
    {
        var userIds = products
            .Select(x => x.CreatedByUser)
            .Distinct()
            .ToList();

        var creators = await _userDao.GetUsersAsync(userIds);

        return creators.ToDictionary(x => x.Id, x => x);
    }

    private static ProductResponseModel CreateResponse(Product product,
        IReadOnlyDictionary<Guid, Account> accountById,
        IReadOnlyDictionary<Guid, User> userById,
        string timeZone)
    {
        var creator = userById[product.CreatedByUser];
        var accountName = accountById[product.AccountId].Name;
        return CreateResponse(product, creator, accountName, timeZone);
    }

    private static ProductResponseModel CreateResponse(Product product, User creator, Name accountName,
        string timeZone)
    {
        var userName = creator.Email;
        var createdAtLocal = product.CreatedAt.ToLocal(timeZone);
        var freezedAtLocal = product.FreezedAt?.ToLocal(timeZone);

        return new ProductResponseModel(product.Id,
            product.AccountId,
            accountName,
            product.Name.Value,
            product.VersioningScheme.Id,
            product.VersioningScheme.Name,
            product.LanguageCode.Value,
            userName,
            createdAtLocal,
            freezedAtLocal);
    }
}