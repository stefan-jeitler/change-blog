using System;

namespace ChangeBlog.Api.Authorization
{
    /// <summary>
    /// Use it with care in situations where no resource id is provided in the request.
    /// For instance, the endpoint to get all products for a user does not contain a resource id in the request.
    /// Therefore, the AuthorizationFilter cannot authorize the request.
    /// You must find all products in all accounts to which the user belongs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class SkipAuthorizationAttribute : Attribute
    {
    }
}