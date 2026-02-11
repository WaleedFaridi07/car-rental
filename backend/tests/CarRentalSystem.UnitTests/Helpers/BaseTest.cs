using CarRentalSystem.Domain.Repositories;
using CarRentalSystem.Domain.Services;
using CarRentalSystem.Domain.Strategies;
using CarRentalSystem.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CarRentalSystem.Tests.Helpers;

public class BaseTest<TService, TImplementation, TRepository> : BaseTest
    where TService : class
    where TImplementation : class, TService
    where TRepository : class
{
    protected TService Sut => ServiceProvider.GetRequiredService<TService>();
    protected TRepository Repository => ServiceProvider.GetRequiredService<TRepository>();

    protected BaseTest()
    {
        var services = new ServiceCollection();

        RegisterServices(services);
        RegisterMocks(services);

        services.Replace<TService, TImplementation>();
        ServiceProvider = services.BuildServiceProvider();
    }
}

public class BaseTest<TService, TImplementation> : BaseTest
    where TService : class
    where TImplementation : class, TService
{
    protected TService Sut => ServiceProvider.GetRequiredService<TService>();

    protected BaseTest()
    {
        var services = new ServiceCollection();

        RegisterServices(services);
        RegisterMocks(services);

        services.Replace<TService, TImplementation>();
        ServiceProvider = services.BuildServiceProvider();
    }
}

public class BaseTest
{
    protected IServiceProvider ServiceProvider { get; set; }

    protected BaseTest()
    {
        var services = new ServiceCollection();

        RegisterServices(services);
        RegisterMocks(services);

        ServiceProvider = services.BuildServiceProvider();
    }

    protected static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton(Substitute.For<IHttpContextAccessor>());
    }

    protected static void RegisterMocks(IServiceCollection services)
    {

        services.AddSingleton(Substitute.For<IRentalService>());
        services.AddSingleton(Substitute.For<IRentalRepository>());

        services.AddSingleton(Substitute.For<IPricingStrategy>());

        services.AddSingleton(Substitute.For<ICarCategoryRepository>());

        RegisterDbContext(services);
    }

    private static void RegisterDbContext(IServiceCollection services)
    {
        var dbContext = Substitute.For<CarRentalDbContext>();

        services.AddSingleton(dbContext);
    }
}