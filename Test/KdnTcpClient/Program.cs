using KdnTcpClient;

using KdnTcpClient.Shared.Interface;
using KdnTcpClient.Util;
using Serilog;


var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configurationBuilder)
    .CreateBootstrapLogger();

try
{
    IHost host = Host.CreateDefaultBuilder(args)
       .ConfigureServices(services =>
       {
           services.AddHostedService<Worker>();
           services.AddSingleton(args);
           services.AddSingleton<IConfiguration>(config => configurationBuilder);

           services.Scan(scan => scan
               .FromAssemblies(AssemblyHelper.GetAllAssemblies(SearchOption.AllDirectories))
               .AddClasses(classes => classes.AssignableTo<ITransientService>())
               .AsSelfWithInterfaces()
               .WithTransientLifetime()
               .AddClasses(classes => classes.AssignableTo<IScopedService>())
               .AsSelfWithInterfaces()
               .WithScopedLifetime()
               .AddClasses(classes => classes.AssignableTo<ISingletonService>())
               .AsSelfWithInterfaces()
               .WithSingletonLifetime());

           services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AssemblyHelper.GetAllAssemblies().ToArray()));
       })
       .UseSerilog((context, configuration) =>
               configuration.ReadFrom.Configuration(context.Configuration))
       .Build();

    host.Run();

}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
        throw;
    if (type.Equals("HostAbortedException", StringComparison.Ordinal))
        throw;
    Log.Fatal(ex, $"{type} Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

