using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.Users;
using ChangeBlog.Application.Models;
using ChangeBlog.Domain;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Tests.TestDoubles
{
    public class UserDaoStub : IUserDao
    {

        public List<User> Users { get; } = new();
        public List<ExternalIdentity> ExternalIdentities { get; } = new();
        public bool ProduceFailureWhileImporting { get; set; }

        public async Task<User> GetUserAsync(Guid userId)
        {
            await Task.Yield();

            return Users.Single(x => x.Id == userId);
        }

        public async Task<IList<User>> GetUsersAsync(IList<Guid> userIds)
        {
            await Task.Yield();

            return Users
                .Where(x => userIds.Any(y => y == x.Id))
                .ToList();
        }

        /// <summary>
        ///     Not properly implemented, but should be enough for use-case tests
        ///     The actual implementation of IUserDao is tested separately.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="limit"></param>
        /// <param name="lastUserId"></param>
        /// <returns></returns>
        public async Task<IList<User>> GetUsersAsync(Guid accountId, ushort limit, Guid? lastUserId)
        {
            await Task.Yield();

            return Users;
        }

        public async Task<Maybe<User>> FindByExternalUserIdAsync(string externalUserId)
        {
            await Task.Yield();

            return ExternalIdentities
                .TryFirst(x => x.ExternalUserId == externalUserId)
                .Bind(eu => Users.TryFirst(u => eu.UserId == u.Id));
        }

        public async Task<Result> AddExternalIdentity(ExternalIdentity externalIdentity)
        {
            await Task.Yield();

            if(ProduceFailureWhileImporting)
                return Result.Failure("Something went wrong.");

            ExternalIdentities.Add(externalIdentity);

            return Result.Success();
        }

        public async Task<Maybe<User>> FindByEmailAsync(string email)
        {
            await Task.Yield();

            var user = Users.SingleOrDefault(x => x.Email == email);

            return user == default
                ? Maybe<User>.None
                : Maybe<User>.From(user);
        }

        public Task<Result> AddAsync(User user)
        {
            Users.Add(user);
            return Task.FromResult(Result.Success());
        }
    }
}
