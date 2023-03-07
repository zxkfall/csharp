using System.Reflection;
using FluentMigrator.Runner;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace FluentNhibernateAndMigration;

public static class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .Build().Run();
    }
}


public class Startup
{
    readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        var connectionString = _configuration.GetValue<string>("DbConnectionString");
        var sessionFactory =
            MySqlServerSessionFactory.AddDbSessionFactory(connectionString);
        services.AddSingleton(sessionFactory);
        services.AddScoped(factory => sessionFactory.OpenSession());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SqlServer support to FluentMigrator
                .AddSqlServer2016()
                // Set the connection string
                .WithGlobalConnectionString(connectionString)
                // Define the assembly containing the migrations
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
        
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
        runner.ListMigrations();
        // runner.MigrateUp(202303072039);
        runner.MigrateDown(202303072039);
    }
}

public static class MySqlServerSessionFactory
{
    public static ISessionFactory AddDbSessionFactory(string connectionString)
    {
        return Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012.ShowSql().ConnectionString(connectionString))
            .Mappings(x => x.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
            .BuildSessionFactory();
    }
}