﻿using ChangeTracker.Application.DataAccess;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ChangeTracker.DataAccess.Postgres.Tests
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void AddDbSession_ResolveUowAndDbAccessor_SameObject()
        {
            // arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddPostgresDbSession(Configuration.ConnectionString);

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            // act
            var dbAccessor = scope.ServiceProvider.GetRequiredService<IDbAccessor>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // assert
            Assert.True(ReferenceEquals(dbAccessor, unitOfWork));
        }
    }
}