namespace CarRentalSystem.Domain.Entities;

public class Rental
{
    public string BookingNumber { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string CustomerSocialSecurityNumber { get; set; } = string.Empty;
    public int CarCategoryId { get; set; }
    public CarCategoryConfig? CarCategory { get; set; }
    public DateTime PickupDateTime { get; set; }
    public int PickupMeterReading { get; set; }
    public DateTime? ReturnDateTime { get; set; }
    public int? ReturnMeterReading { get; set; }
    public decimal? TotalPrice { get; set; }
}
