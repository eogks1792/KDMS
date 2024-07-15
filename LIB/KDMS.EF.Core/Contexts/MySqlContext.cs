using KDMS.EF.Core.Infrastructure.Reverse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KDMS.EF.Core.Contexts;

public class MySqlContext : KdmsContext
{
    public MySqlContext(IConfiguration configuration) : base(configuration)
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string? connectionString = _configuration.GetConnectionString("Server");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}
