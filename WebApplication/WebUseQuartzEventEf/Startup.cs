using Microsoft.EntityFrameworkCore;
using WebUseQuartzEventEf.Domain;

namespace WebUseQuartzEventEf;

public class Startup
{
    //default private
    readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // Learn configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        services.AddDbContext<AccountContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlServer(_configuration.GetValue<string>("DbConnectionString"));
        });
        services.AddMiddlewares(_configuration);
        services.AddQuartz(_configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        
        app.UseRouting();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseEndpoints(endpoints => { endpoints.MapGet("/hello", () => "Hello World!"); });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseHealthCheck();
    }
}