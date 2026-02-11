using CarRentalSystem.Domain.Strategies;

namespace CarRentalSystem.Application.Strategies;

public class CombiPricingStrategy : IPricingStrategy
{
    public decimal CalculatePrice(int days, int km, decimal baseDayRental, decimal baseKmPrice)
    {
        return baseDayRental * days * 1.3m + baseKmPrice * km;
    }
}
