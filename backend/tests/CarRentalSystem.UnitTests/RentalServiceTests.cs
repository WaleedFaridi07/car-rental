using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CarRentalSystem.Tests;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _rentalRepositoryMock;
    private readonly Mock<ICarCategoryRepository> _categoryRepositoryMock;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _categoryRepositoryMock = new Mock<ICarCategoryRepository>();
        _service = new RentalService(_rentalRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public void GenerateBookingNumber_ReturnsCorrectFormat()
    {
        // Act
        var bookingNumber = RentalService.GenerateBookingNumber();

        // Assert
        bookingNumber.Should().StartWith("BK");
        bookingNumber.Should().HaveLength(17);
    }

    [Fact]
    public async Task RegisterPickupAsync_SuccessfulRegistration()
    {
        // Arrange
        var dto = new PickupRegistrationDto
        {
            BookingNumber = "BK202602101230123",
            RegistrationNumber = "ABC123",
            CustomerSocialSecurityNumber = "19900101-1234",
            CarCategoryId = 1,
            PickupDateTime = DateTime.Now,
            PickupMeterReading = 10000
        };

        _rentalRepositoryMock.Setup(r => r.BookingNumberExistsAsync(dto.BookingNumber))
            .ReturnsAsync(false);

        // Act
        var result = await _service.RegisterPickupAsync(dto);

        // Assert
        result.Should().Be(dto.BookingNumber);
        _rentalRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Rental>()), Times.Once);
    }

    [Fact]
    public async Task RegisterPickupAsync_ThrowsException_WhenBookingNumberExists()
    {
        // Arrange
        var dto = new PickupRegistrationDto
        {
            BookingNumber = "BK202602101230123"
        };

        _rentalRepositoryMock.Setup(r => r.BookingNumberExistsAsync(dto.BookingNumber))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterPickupAsync(dto));
    }

    [Fact]
    public async Task RegisterReturnAsync_SuccessfulReturn()
    {
        // Arrange
        var rental = new Rental
        {
            BookingNumber = "BK202602101230123",
            CarCategoryId = 1,
            PickupDateTime = DateTime.Now.AddDays(-2),
            PickupMeterReading = 10000
        };

        var category = new CarCategoryConfig
        {
            Id = 1,
            Category = CarCategory.SmallCar,
            BaseDayRental = 300,
            BaseKmPrice = 0
        };

        var dto = new ReturnRegistrationDto
        {
            BookingNumber = "BK202602101230123",
            ReturnDateTime = DateTime.Now,
            ReturnMeterReading = 10200
        };

        _rentalRepositoryMock.Setup(r => r.GetByBookingNumberAsync(dto.BookingNumber))
            .ReturnsAsync(rental);
        _categoryRepositoryMock.Setup(c => c.GetByIdAsync(rental.CarCategoryId))
            .ReturnsAsync(category);

        // Act
        var result = await _service.RegisterReturnAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.BookingNumber.Should().Be(dto.BookingNumber);
        result.KmDriven.Should().Be(200);
        _rentalRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Rental>()), Times.Once);
    }

    [Fact]
    public async Task RegisterReturnAsync_ThrowsException_WhenRentalNotFound()
    {
        // Arrange
        var dto = new ReturnRegistrationDto
        {
            BookingNumber = "BK202602101230123"
        };

        _rentalRepositoryMock.Setup(r => r.GetByBookingNumberAsync(dto.BookingNumber))
            .ReturnsAsync((Rental?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterReturnAsync(dto));
    }

    [Fact]
    public async Task RegisterReturnAsync_ThrowsException_WhenAlreadyReturned()
    {
        // Arrange
        var rental = new Rental
        {
            BookingNumber = "BK202602101230123",
            ReturnDateTime = DateTime.Now.AddDays(-1)
        };

        var dto = new ReturnRegistrationDto
        {
            BookingNumber = "BK202602101230123"
        };

        _rentalRepositoryMock.Setup(r => r.GetByBookingNumberAsync(dto.BookingNumber))
            .ReturnsAsync(rental);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterReturnAsync(dto));
    }

    [Fact]
    public async Task RegisterReturnAsync_ThrowsException_WhenMeterReadingInvalid()
    {
        // Arrange
        var rental = new Rental
        {
            BookingNumber = "BK202602101230123",
            PickupMeterReading = 10000
        };

        var dto = new ReturnRegistrationDto
        {
            BookingNumber = "BK202602101230123",
            ReturnMeterReading = 9000
        };

        _rentalRepositoryMock.Setup(r => r.GetByBookingNumberAsync(dto.BookingNumber))
            .ReturnsAsync(rental);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterReturnAsync(dto));
    }

    [Theory]
    [InlineData("2026-02-01 10:00", "2026-02-01 15:00", 1)] // Same day
    [InlineData("2026-02-01 10:00", "2026-02-02 09:00", 1)] // Less than 24 hours
    [InlineData("2026-02-01 10:00", "2026-02-02 11:00", 2)] // More than 24 hours
    [InlineData("2026-02-01 10:00", "2026-02-04 10:00", 3)] // Exactly 3 days
    public void CalculateDays_ReturnsCorrectDays(string pickupStr, string returnStr, int expectedDays)
    {
        // Arrange
        var pickup = DateTime.Parse(pickupStr);
        var returnDate = DateTime.Parse(returnStr);

        // Act
        var days = RentalService.CalculateDays(pickup, returnDate);

        // Assert
        days.Should().Be(expectedDays);
    }

    [Fact]
    public void CalculatePrice_SmallCar_ReturnsCorrectPrice()
    {
        // Act
        var price = _service.CalculatePrice(CarCategory.SmallCar, 3, 0, 300, 0);

        // Assert
        price.Should().Be(900m);
    }

    [Fact]
    public void CalculatePrice_Combi_ReturnsCorrectPrice()
    {
        // Act
        var price = _service.CalculatePrice(CarCategory.Combi, 2, 100, 500, 2);

        // Assert
        price.Should().Be(1500m);
    }

    [Fact]
    public void CalculatePrice_Truck_ReturnsCorrectPrice()
    {
        // Act
        var price = _service.CalculatePrice(CarCategory.Truck, 3, 150, 800, 3);

        // Assert
        price.Should().Be(4275m);
    }
}
