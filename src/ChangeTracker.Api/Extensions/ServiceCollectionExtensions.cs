using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using ChangeTracker.Application.UseCases.Queries.GetAccounts;
using ChangeTracker.Application.UseCases.Queries.GetProducts;
using ChangeTracker.Application.UseCases.Queries.GetRoles;
using ChangeTracker.Application.UseCases.Queries.GetUsers;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProductUseCase(this IServiceCollection services)
        {
            return services
                .AddScoped<IAddProduct, AddProductInteractor>()
                .AddScoped<ICloseProduct, CloseProductInteractor>()
                .AddScoped<IGetProducts, GetProductsInteractor>()
                .AddScoped<IGetRoles, GetRolesInteractor>()
                .AddScoped<IGetAccounts, GetAccountsInteractor>()
                .AddScoped<IGetUsers, GetUsersInteractor>();
        }
    }
}