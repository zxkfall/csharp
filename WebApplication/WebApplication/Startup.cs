namespace WebApplication;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    
    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // services.AddDbContextFactory<WebApplicationContext>(
        //     options => options.UseSqlServer(Configuration["DBConnectionString"]), ServiceLifetime.Scoped);
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureApplicationInsights(Configuration);

        services.AddHealthCheckServices(Configuration);
        services.AddMiddlewares(Configuration);
        services.AddHostedServices();
        services.AddDomainServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHealthCheck();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}