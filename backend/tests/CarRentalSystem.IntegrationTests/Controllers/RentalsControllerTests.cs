using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using CarRentalSystem.Domain.Dtos;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.IntegrationTests.Helpers;

namespace CarRentalSystem.IntegrationTests.Controllers;

public class RentalsControllerTests(CarRentalSystemApiFactory factory) : IClassFixture<CarRentalSystemApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly Faker<PickupRegistrationDto> _bookingFaker = new Faker<PickupRegistrationDto>()
        .RuleFor(x => x.BookingNumber, f => f.Random.AlphaNumeric(6))
        .RuleFor(x => x.RegistrationNumber, f => f.Vehicle.Vin())
        .RuleFor(x => x.CustomerSocialSecurityNumber, f => f.Random.Replace("###-##-####"))
        .RuleFor(x => x.CarCategoryId, f => f.Random.Int(1, 2))
        .RuleFor(x => x.PickupDateTime, f => f.Date.Recent())
        .RuleFor(x => x.PickupMeterReading, f => f.Random.Int(10000, 50000));
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    
    public class BookingResponseDto
    {
        public string BookingNumber { get; set; } = string.Empty;
    }
    
    [Fact]
    public async Task RegisterPickup_WhenCalled_ShouldReturnBookingNumber()
    {
        // Arrange
        var request = _bookingFaker.Generate();
        var response = await _client.PostAsJsonAsync("/api/rentals/pickup", request);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var bookingNumber = doc.RootElement.GetProperty("value").GetString();

        // Assert
        Assert.Equal(request.BookingNumber, bookingNumber);
    }
    
    
    [Fact]
    public async Task GetAllRentals_WhenCalled_ShouldReturnAllRentals()
    {
        // Arrange
        await _client.GetAsync("/api/rentals");

        // Act
        var rentals = await _client.GetFromJsonAsync<List<Rental>>("/api/rentals", _jsonOptions);

        // Assert
        Assert.NotNull(rentals);
        Assert.NotEmpty(rentals);
        Assert.Equal(2,  rentals.Count);
    }
    
    [Fact]
    public async Task GetRentalByBookingNumber_WhenCalled_ShouldReturnBooking()
    {
        // Arrange
        const string bookingNumber = "BK0001";    
        await _client.GetAsync("/api/rentals"+$"/{bookingNumber}");

        // Act
        var rental = await _client.GetFromJsonAsync<Rental>("/api/rentals"+$"/{bookingNumber}", _jsonOptions);

        // Assert
        Assert.NotNull(rental);
        Assert.Equal("REG123",  rental.RegistrationNumber);
    }
    
    [Fact]
    public async Task GetRentalByBookingNumber_WhenCalled_ShouldReturnNotFound()
    {
        // Arrange
        const string bookingNumber = "BK0005";    

        // Act
        var response = await _client.GetAsync("/api/rentals"+$"/{bookingNumber}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}