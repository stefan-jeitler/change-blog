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
    public class GetProductsInteractor : IGetProducts
    {
        private readonly IProductDao _productDao;
        private readonly IUserDao _userDao;

        public GetProductsInteractor(IProductDao productDao, IUserDao userDao)
        {
            _productDao = productDao ?? throw new ArgumentNullException(nameof(productDao));
            _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        }

        public async Task<IList<ProductResponseModel>> ExecuteAsync(
            ProductQueryRequestModel requestModel)
        {
            var productQuerySettings = new ProductQuerySettings(requestModel.AccountId,
                requestModel.UserId,
                requestModel.LastProductId,
                requestModel.Limit,
                requestModel.IncludeClosedProducts);

            var products = await _productDao.GetProductsAsync(productQuerySettings);
            var currentUser = await _userDao.GetUserAsync(requestModel.UserId);

            var users = await GetProductUsersAsync(products);
            var userById = users.ToDictionary(x => x.Id, x => x);

            return products
                .Select(x => CreateResponse(x, userById, currentUser.TimeZone))
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

        private async Task<IList<User>> GetProductUsersAsync(IEnumerable<Product> products)
        {
            var userIds = products
                .Select(x => x.CreatedByUser)
                .Distinct()
                .ToList();

            return await _userDao.GetUsersAsync(userIds);
        }

        private static ProductResponseModel CreateResponse(Product product,
            IReadOnlyDictionary<Guid, User> userById,
            string timeZone)
        {
            var user = userById[product.CreatedByUser];
            return CreateResponse(product, user, timeZone);
        }

        private static ProductResponseModel CreateResponse(Product product, User user,
            string timeZone)
        {
            var userName = user.Email;
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