// using Ef.Domain;
// using Microsoft.EntityFrameworkCore;

using EfExample.Domain;
using EfExample.Service;
using Microsoft.EntityFrameworkCore;

// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
//
// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddDbContext<AccountContext>(optionsBuilder =>
//     // optionsBuilder.UseSqlServer("Server=localhost;Database=test;Trusted_Connection=True;"));// certificate problem A connection was successfully established with the server, but then an error occurred during the login process. (provider: SSL Provider, error: 0 - 证书链是由不受信任的颁发机构颁发的gi
//     optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=test;Integrated Security=SSPI;Encrypt=True;Trust Server Certificate=True;"));
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
//
// app.UseAuthorization();
//
// app.MapControllers();
//
// app.Run();

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).Build().Run();
    }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<AccountContext>(optionsBuilder =>
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=test;Integrated Security=SSPI;Encrypt=True;Trust Server Certificate=True;"));
        services.AddScoped<IAccountService, AccountService>();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // app.UseAuthorization();
        app.UseRouting();
        app.UseEndpoints(e => e.MapControllers());

        // app.MapControllers();
    }
}