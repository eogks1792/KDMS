using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using KDMS.EF.Core.Infrastructure.Reverse;

namespace KDMS.EF.Core.Contexts;

public class SqlServerContext : KdmsContext
{
    public SqlServerContext(IConfiguration configuration) : base(configuration)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string? connectionString = _configuration.GetConnectionString("Server");
            optionsBuilder.UseSqlServer(connectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds));
        }
    }

}
