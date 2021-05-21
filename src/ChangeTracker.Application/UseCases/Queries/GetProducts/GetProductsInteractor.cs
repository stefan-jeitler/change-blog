using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Domain;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetProducts
{
    public class GetProductsInteractor : IGetAccountProducts, IGetUserProducts, IGetProduct
    {
        private readonly IProductDao _productDao;
        private readonly IUserDao _userDao;

        public GetProductsInteractor(IProductDao productDao, IUserDao userDao)
        {
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        }

        public async Task<IList<ProductResponseModel>> ExecuteAsync(
            AccountProductQueryRequestModel requestModel)
        {
            var productQuerySettings = new AccountProductsQuerySettings(requestModel.AccountId,
                requestModel.UserId,
                requestModel.LastProductId,
                requestModel.Limit,
                requestModel.IncludeClosedProducts);

            var products = await _productDao.GetAccountProductsAsync(productQuerySettings);
            var currentUser = await _userDao.GetUserAsync(requestModel.UserId);
            var creatorById = await GetProductCreatorsAsync(products);

            return products
                .Select(x => CreateResponse(x, creatorById, currentUser.TimeZone))
                .ToList();
        }

        public async Task<IList<ProductResponseModel>> ExecuteAsync(UserProductQueryRequestModel requestModel)
        {
            var querySettings = new UserProductsQuerySettings(requestModel.UserId,
                requestModel.LastProductId,
                requestModel.Limit,
                requestModel.IncludeClosedProducts);

            var products = await _productDao.GetUserProductsAsync(querySettings);
            var currentUser = await _userDao.GetUserAsync(requestModel.UserId);
            var creatorById = await GetProductCreatorsAsync(products);

            return products
                .Select(x => CreateResponse(x, creatorById, currentUser.TimeZone))
                .ToList();
        }

        public async Task<Maybe<ProductResponseModel>> ExecuteAsync(Guid userId, Guid productId)
        {
            var product = await _productDao.FindProductAsync(productId);
            var currentUser = await _userDao.GetUserAsync(userId);

            return await product
                .Map(async p => new {User = await _userDao.GetUserAsync(p.CreatedByUser), Product = p})
                .Map(x => CreateResponse(x.Product, x.User, currentUser.TimeZone));
        }

        private async Task<IReadOnlyDictionary<Guid, User>> GetProductCreatorsAsync(IEnumerable<Product> products)
        {
            var userIds = products
                .Select(x => x.CreatedByUser)
                .Distinct()
                .ToList();

            var creators =  await _userDao.GetUsersAsync(userIds);

            return creators.ToDictionary(x => x.Id, x => x);
        }

        private static ProductResponseModel CreateResponse(Product product,
            IReadOnlyDictionary<Guid, User> userById,
            string timeZone)
        {
            var creator = userById[product.CreatedByUser];
            return CreateResponse(product, creator, timeZone);
        }

        private static ProductResponseModel CreateResponse(Product product, User creator,
            string timeZone)
        {
            var userName = creator.Email;
            var createdAtLocal = product.CreatedAt.ToLocal(timeZone);
            var closedAtLocal = product.ClosedAt?.ToLocal(timeZone);

            return new ProductResponseModel(product.Id,
                product.AccountId,
                product.Name.Value,
                product.VersioningScheme.Id,
                product.VersioningScheme.Name,
                userName,
                createdAtLocal,
                closedAtLocal);
        }
    }
}