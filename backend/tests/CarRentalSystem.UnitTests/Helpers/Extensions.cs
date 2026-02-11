using Microsoft.Extensions.DependencyInjection;


namespace CarRentalSystem.Tests.Helpers;

public static class Extensions
{
    public static void Replace<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        // Remove Mock for the service that will be tested
        var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TService));
        if (serviceDescriptor != null)
        {
            services.Remove(serviceDescriptor);
        }

        // Add the implementation that will be tested
        services.AddScoped<TService, TImplementation>();
    }
}