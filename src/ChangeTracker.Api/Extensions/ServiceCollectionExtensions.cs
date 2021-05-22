using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.Application.UseCases.Commands.AddVersion;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using ChangeTracker.Application.UseCases.Queries.GetAccounts;
using ChangeTracker.Application.UseCases.Queries.GetCompleteVersions;
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
            services
                .AddScoped<IAddProduct, AddProductInteractor>()
                .AddScoped<ICloseProduct, CloseProductInteractor>()
                .AddScoped<IGetAccountProducts, GetProductsInteractor>()
                .AddScoped<IGetRoles, GetRolesInteractor>()
                .AddScoped<IGetAccounts, GetAccountsInteractor>()
                .AddScoped<IGetUsers, GetUsersInteractor>()
                .AddScoped<IGetUserProducts, GetProductsInteractor>()
                .AddScoped<IGetProduct, GetProductsInteractor>()
                .AddScoped<IGetAccountProducts, GetProductsInteractor>();

            services
                .AddScoped<IAddVersion, AddVersionInteractor>()
                .AddScoped<IAddCompleteVersion, AddCompleteVersionInteractor>()
                .AddScoped<IGetCompleteVersion, GetCompleteVersionsInteractor>()
                .AddScoped<IGetCompleteVersions, GetCompleteVersionsInteractor>();

            return services;
        }
    }
}