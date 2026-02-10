using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CarRentalSystem.CrossCutting.Extensions;

public static class EnvironmentExtensions
{
    public static bool IsTest(this IWebHostEnvironment env)
    {
        return env.IsEnvironment("Test");
    }
}