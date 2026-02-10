using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.IntegrationTests.Helpers;

namespace CarRentalSystem.IntegrationTests.Controllers;

public class CategoriesControllerTests(CarRentalSystemApiFactory factory) : IClassFixture<CarRentalSystemApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    
    [Fact]
    public async Task GetAllCategories_WhenCalled_ShouldReturnCategories()
    {
        // Arrange
        await _client.GetAsync("/api/categories");

        // Act
        var categories = await _client.GetFromJsonAsync<List<CarCategoryConfig>>("/api/categories", _jsonOptions);

        // Assert
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
        Assert.Contains(categories, r => r.Category == categories.First().Category);
    }
}