using CarRentalSystem.Domain.Strategies;

namespace CarRentalSystem.Application.Strategies;

public class SmallCarPricingStrategy : IPricingStrategy
{
    public decimal CalculatePrice(int days, int km, decimal baseDayRental, decimal baseKmPrice)
    {
        return baseDayRental * days;
    }
}
