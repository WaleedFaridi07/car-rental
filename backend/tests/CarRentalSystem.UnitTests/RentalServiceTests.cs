using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Dtos;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Repositories;
using CarRentalSystem.Domain.Services;
using CarRentalSystem.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CarRentalSystem.Tests;

public class RentalServiceTests : BaseTest<IRentalService, RentalService, IRentalRepository>
{
    [Fact]
    public void GenerateBookingNumber_ReturnsCorrectFormat()
    {
        // Act
        var bookingNumber = Sut.GenerateBookingNumber();

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
        

        Repository.BookingNumberExistsAsync(dto.BookingNumber).Returns(false);

        // Act
        var result = await Sut.RegisterPickupAsync(dto);

        // Assert
        result.Should().Be(dto.BookingNumber);
        await Repository.Received(1).AddAsync(Arg.Is<Rental>(r =>
            r.BookingNumber == dto.BookingNumber &&
            r.RegistrationNumber == dto.RegistrationNumber &&
            r.CustomerSocialSecurityNumber == dto.CustomerSocialSecurityNumber &&
            r.CarCategoryId == dto.CarCategoryId &&
            r.PickupDateTime == dto.PickupDateTime &&
            r.PickupMeterReading == dto.PickupMeterReading));
    }

    [Fact]
    public async Task RegisterPickupAsync_ThrowsException_WhenBookingNumberExists()
    {
        // Arrange
        var dto = new PickupRegistrationDto
        {
            BookingNumber = "BK202602101230123"
        };
        
        Repository.BookingNumberExistsAsync(dto.BookingNumber).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.RegisterPickupAsync(dto));
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
        
        Repository.GetByBookingNumberAsync(dto.BookingNumber).Returns(rental);
        var carCategoryRepository = ServiceProvider.GetRequiredService<ICarCategoryRepository>();
        carCategoryRepository.GetByIdAsync(rental.CarCategoryId).Returns(category);

        // Act
        var result = await Sut.RegisterReturnAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.BookingNumber.Should().Be(dto.BookingNumber);
        result.KmDriven.Should().Be(200);
    }

    [Fact]
    public async Task RegisterReturnAsync_ThrowsException_WhenRentalNotFound()
    {
        // Arrange
        var dto = new ReturnRegistrationDto
        {
            BookingNumber = "BK202602101230123"
        };
        
        Repository.GetByBookingNumberAsync(dto.BookingNumber).Returns((Rental?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.RegisterReturnAsync(dto));
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
        
        Repository.GetByBookingNumberAsync(dto.BookingNumber).Returns(rental);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.RegisterReturnAsync(dto));
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
        
        Repository.GetByBookingNumberAsync(dto.BookingNumber).Returns(rental);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.RegisterReturnAsync(dto));
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
        var price = Sut.CalculatePrice(CarCategory.SmallCar, 3, 0, 300, 0);

        // Assert
        price.Should().Be(900m);
    }

    [Fact]
    public void CalculatePrice_Combi_ReturnsCorrectPrice()
    {
        // Act
        var price = Sut.CalculatePrice(CarCategory.Combi, 2, 100, 500, 2);

        // Assert
        price.Should().Be(1500m);
    }

    [Fact]
    public void CalculatePrice_Truck_ReturnsCorrectPrice()
    {
        // Act
        var price = Sut.CalculatePrice(CarCategory.Truck, 3, 150, 800, 3);

        // Assert
        price.Should().Be(4275m);
    }
}
