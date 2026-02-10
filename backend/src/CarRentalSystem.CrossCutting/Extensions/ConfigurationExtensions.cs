using Microsoft.Extensions.Configuration;

namespace CarRentalSystem.CrossCutting.Extensions;

public static class ConfigurationExtensions
{
    private const string DefaultConnectionName = "DefaultConnection";
    
    public static string GetConnectionString(this IConfiguration config)
    {
        // Try to get connection string from standard configuration
        var connectionString = config.GetConnectionString(DefaultConnectionName);
        return connectionString!;
        // Get connection details from AWS Secrets Manager
        //BuildConnectionStringFromExternalSecrets(config);
    }
    

}