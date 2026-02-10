using CarRentalSystem.Application.Strategies;
using FluentAssertions;

namespace CarRentalSystem.Tests;

public class PricingStrategyTests
{
    [Fact]
    public void SmallCarPricingStrategy_CalculatesCorrectPrice()
    {
        // Arrange
        var strategy = new SmallCarPricingStrategy();
        const decimal baseDayRental = 300m;
        const int days = 3;

        // Act
        var price = strategy.CalculatePrice(days, 0, baseDayRental, 0);

        // Assert
        price.Should().Be(900m);
    }

    [Fact]
    public void CombiPricingStrategy_CalculatesCorrectPrice()
    {
        // Arrange
        var strategy = new CombiPricingStrategy();
        const decimal baseDayRental = 500m;
        const decimal baseKmPrice = 2m;
        const int days = 2;
        const int km = 100;

        // Act
        var price = strategy.CalculatePrice(days, km, baseDayRental, baseKmPrice);

        // Assert
        price.Should().Be(1500m);
    }

    [Fact]
    public void TruckPricingStrategy_CalculatesCorrectPrice()
    {
        // Arrange
        var strategy = new TruckPricingStrategy();
        const decimal baseDayRental = 800m;
        const decimal baseKmPrice = 3m;
        const int days = 3;
        const int km = 150;

        // Act
        var price = strategy.CalculatePrice(days, km, baseDayRental, baseKmPrice);

        // Assert
        price.Should().Be(4275m);
    }
}
