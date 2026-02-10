using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Entities;

public class CarCategoryConfig
{
    public int Id { get; set; }
    public CarCategory Category { get; set; }
    public decimal BaseDayRental { get; set; }
    public decimal BaseKmPrice { get; set; }
}
