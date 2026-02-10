using CarRentalSystem.CrossCutting.Extensions;

namespace CarRentalSystem.API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            await ConfigureServices(builder);
            var webApp = await ConfigureApplication(builder);
            await webApp.RunAsync();
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Application startup failed: {ex.Message}");
            throw;
        }
    }

    private static async Task ConfigureServices(WebApplicationBuilder builder)
    {
        
        ConfigureCors(builder);
        AddCommonServices(builder);
        await AddDomainServices(builder);
    }
    
    private static void ConfigureCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            var origins = builder.Configuration.GetSection("AppSettings:AllowedOrigins").Get<string[]>() ?? [];

            options.AddDefaultPolicy(x =>
            {
                if (!builder.Environment.IsTest())
                {
                    x.WithOrigins(origins);
                    x.AllowAnyHeader();
                    x.AllowAnyMethod();
                    return;
                }

                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.AllowAnyOrigin();
            });
        });
    }
    
    private static void AddCommonServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSwaggerGen();
    }
    
    private static Task AddDomainServices(WebApplicationBuilder builder)
    {
        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddRepositories();
        builder.Services.AddDbContext(builder.Configuration);
        return Task.CompletedTask;
    }

    private static async Task<WebApplication> ConfigureApplication(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        ConfigureMiddlewarePipeline(app);

        // Only run database migrations and seed test data in test environment
        if (builder.Environment.IsTest())
        {
            await app.MigrateDatabaseAsync();
        }
        return app;
    }
    private static void ConfigureMiddlewarePipeline(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseCors();
    }
}