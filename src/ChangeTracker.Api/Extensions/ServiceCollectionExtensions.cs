using ChangeTracker.Application.UseCases.Commands.AddCompleteVersion;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.Application.UseCases.Commands.AddVersion;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using ChangeTracker.Application.UseCases.Commands.DeleteVersion;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
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
        public static IServiceCollection AddUseCases(this IServiceCollection services) =>
            services
                .AddAccountUseCases()
                .AddProductUseCases()
                .AddVersionUseCases();

        public static IServiceCollection AddAccountUseCases(this IServiceCollection services) =>
            services
                .AddScoped<IGetRoles, GetRolesInteractor>()
                .AddScoped<IGetAccounts, GetAccountsInteractor>()
                .AddScoped<IGetUsers, GetUsersInteractor>();

        public static IServiceCollection AddProductUseCases(this IServiceCollection services) =>
            services
                .AddScoped<IAddProduct, AddProductInteractor>()
                .AddScoped<ICloseProduct, CloseProductInteractor>()
                .AddScoped<IGetUserProducts, GetProductsInteractor>()
                .AddScoped<IGetProduct, GetProductsInteractor>()
                .AddScoped<IGetAccountProducts, GetProductsInteractor>();

        public static IServiceCollection AddVersionUseCases(this IServiceCollection services) =>
            services
                .AddScoped<IAddVersion, AddVersionInteractor>()
                .AddScoped<IAddCompleteVersion, AddCompleteVersionInteractor>()
                .AddScoped<IGetCompleteVersion, GetCompleteVersionsInteractor>()
                .AddScoped<IGetCompleteVersions, GetCompleteVersionsInteractor>()
                .AddScoped<IReleaseVersion, ReleaseVersionInteractor>()
                .AddScoped<IDeleteVersion, DeleteVersionInteractor>()
                .AddScoped<IAddOrUpdateVersion, AddOrUpdateVersionInteractor>();
    }
}