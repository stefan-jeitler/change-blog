using ChangeTracker.Application.UseCases.Commands.AddChangeLogLine;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion;
using ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using ChangeTracker.Application.UseCases.Commands.DeleteAllPendingChangeLogLines;
using ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine;
using ChangeTracker.Application.UseCases.Commands.DeleteVersion;
using ChangeTracker.Application.UseCases.Commands.Issues.AddChangeLogLineIssue;
using ChangeTracker.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;
using ChangeTracker.Application.UseCases.Commands.Labels.AddChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Commands.MakeAllChangeLogLinesPending;
using ChangeTracker.Application.UseCases.Commands.MakeChangeLogLinePending;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
using ChangeTracker.Application.UseCases.Queries.GetAccounts;
using ChangeTracker.Application.UseCases.Queries.GetChangeLogLine;
using ChangeTracker.Application.UseCases.Queries.GetIssues;
using ChangeTracker.Application.UseCases.Queries.GetLabels;
using ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogLine;
using ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogs;
using ChangeTracker.Application.UseCases.Queries.GetProducts;
using ChangeTracker.Application.UseCases.Queries.GetRoles;
using ChangeTracker.Application.UseCases.Queries.GetUsers;
using ChangeTracker.Application.UseCases.Queries.GetVersions;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services) =>
            services
                .AddAccountUseCases()
                .AddProductUseCases()
                .AddVersionUseCases()
                .AddPendingChangeLogLineUseCases()
                .AddChangeLogLineUseCases()
                .AddLabelUseCases()
                .AddIssueUseCases();

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
                .AddScoped<IAddVersion, AddOrUpdateVersionInteractor>()
                .AddScoped<IAddOrUpdateVersion, AddOrUpdateVersionInteractor>()
                .AddScoped<IGetVersion, GetVersionsInteractor>()
                .AddScoped<IGetVersions, GetVersionsInteractor>()
                .AddScoped<IReleaseVersion, ReleaseVersionInteractor>()
                .AddScoped<IDeleteVersion, DeleteVersionInteractor>();

        public static IServiceCollection AddPendingChangeLogLineUseCases(this IServiceCollection services) =>
            services
                .AddScoped<IAddPendingChangeLogLine, AddPendingChangeLogLineInteractor>()
                .AddScoped<IGetPendingChangeLogLine, GetPendingChangeLogLineInteractor>()
                .AddScoped<IGetPendingChangeLogs, GetPendingChangeLogsInteractor>()
                .AddScoped<IAssignPendingLineToVersion, AssignPendingLineToVersionInteractor>()
                .AddScoped<IAssignAllPendingLinesToVersion, AssignAllPendingLinesToVersionInteractor>()
                .AddScoped<IDeleteAllPendingChangeLogLines, DeleteAllPendingChangeLogLinesInteractor>();

        public static IServiceCollection AddChangeLogLineUseCases(this IServiceCollection services) =>
            services
                .AddScoped<IMakeChangeLogLinePending, MakeChangeLogLinePendingInteractor>()
                .AddScoped<IMakeAllChangeLogLinesPending, MakeAllChangeLogLinesPendingInteractor>()
                .AddScoped<IGetChangeLogLine, GetChangeLogLineInteractor>()
                .AddScoped<IDeleteChangeLogLine, DeleteChangeLogLineInteractor>()
                .AddScoped<IAddChangeLogLine, AddChangeLogLineInteractor>();

        public static IServiceCollection AddLabelUseCases(this IServiceCollection services) =>
            services
                .AddScoped<IGetLabels, GetLabelsInteractor>()
                .AddScoped<IDeleteChangeLogLineLabel, DeleteChangeLogLineLabelInteractor>()
                .AddScoped<IAddChangeLogLineLabel, AddChangeLogLineLabelInteractor>();

        public static IServiceCollection AddIssueUseCases(this IServiceCollection services) =>
            services
                .AddScoped<IGetIssues, GetIssuesInteractor>()
                .AddScoped<IDeleteChangeLogLineIssue, DeleteChangeLogLineIssueInteractor>()
                .AddScoped<IAddChangeLogLineIssue, AddChangeLogLineIssueInteractor>();
    }
}