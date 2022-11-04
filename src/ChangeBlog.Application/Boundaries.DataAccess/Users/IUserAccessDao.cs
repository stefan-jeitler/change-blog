using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain;

namespace ChangeBlog.Application.Boundaries.DataAccess.Users;

public interface IUserAccessDao
{
    Task<IEnumerable<Role>> GetAccountRolesAsync(Guid userId, Guid accountId);
    Task<AccountProductRolesDto> GetRolesByProductIdAsync(Guid userId, Guid productId);
    Task<AccountProductRolesDto> GetRolesByVersionIdAsync(Guid userId, Guid versionId);
    Task<AccountProductRolesDto> GetRolesByChangeLogLineIdAsync(Guid userId, Guid changeLogLineId);
}