using CarRentalSystem.Domain.Strategies;

namespace CarRentalSystem.Application.Strategies;

public class TruckPricingStrategy : IPricingStrategy
{
    public decimal CalculatePrice(int days, int km, decimal baseDayRental, decimal baseKmPrice)
    {
        return baseDayRental * days * 1.5m + baseKmPrice * km * 1.5m;
    }
}
