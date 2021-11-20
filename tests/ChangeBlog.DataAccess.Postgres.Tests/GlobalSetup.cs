using System.Runtime.CompilerServices;

namespace ChangeBlog.DataAccess.Postgres.Tests;

public static class GlobalSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        DapperTypeHandlers.Add();
    }
}