
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Infrastructure.Reverse;

namespace KDMS.EF.Core.Extensions;

public static class DbContextConfigurationExtensions
{
    public static void AddServerAccessServices(this IServiceCollection self, string connectionString, string dbProvider = "SqlServer", int maxRetryCount = 3)
    {
        if (dbProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
        {
            self.AddDbContext<KdmsContext, MySqlContext>
                (options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                            x => x.EnableRetryOnFailure(maxRetryCount: maxRetryCount)));
        }
        else
        {
            self.AddDbContext<KdmsContext, SqlServerContext>
                (options => options.UseSqlServer(connectionString,
                            x => x.EnableRetryOnFailure(maxRetryCount: maxRetryCount)));
        }
    }

    public static void AddServerDBServices(this IServiceCollection self, string dbProvider = "SqlServer")
    {
        if (dbProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
        {
            self.AddSingleton<KdmsContext, MySqlContext>();
        }
        else
        {
            self.AddSingleton<KdmsContext, SqlServerContext>();
        }
    }

    public static KdmsContext CreateServerContext(IConfiguration configuration)
    {
        string dbProvider = configuration.GetSection("DbProvider")?.Value ?? "SqlServer";

        if (dbProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
        {
            return new MySqlContext(configuration);
        }
        else
        {
            return new SqlServerContext(configuration);
        }
    }
}

