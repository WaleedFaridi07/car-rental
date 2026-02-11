namespace CarRentalSystem.Domain.Strategies;

public interface IPricingStrategy
{
    decimal CalculatePrice(int days, int km, decimal baseDayRental, decimal baseKmPrice);
}