using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Authorization;

namespace ChangeBlog.Application.DataAccess.Users
{
    public interface IUserAccessDao
    {
        Task<IEnumerable<Role>> GetAccountRolesAsync(Guid accountId, Guid userId);
        Task<AccountProductRolesDto> GetRolesByProductIdAsync(Guid userId, Guid productId);
        Task<AccountProductRolesDto> GetRolesByVersionIdAsync(Guid userId, Guid versionId);
        Task<AccountProductRolesDto> GetRolesByChangeLogLineIdAsync(Guid userId, Guid changeLogLineId);
    }
}
