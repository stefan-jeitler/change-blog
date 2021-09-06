using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeBlog.Application.DataAccess.ExternalIdentity
{
    public record UserInfo(string Id, 
        string FullName,
        string FirstName,
        string LastName,
        string Email,
        string IdentityProvider);
}
