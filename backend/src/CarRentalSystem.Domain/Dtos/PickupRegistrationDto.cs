namespace CarRentalSystem.Domain.Dtos;

public class PickupRegistrationDto
{
    public string BookingNumber { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string CustomerSocialSecurityNumber { get; set; } = string.Empty;
    public int CarCategoryId { get; set; }
    public DateTime PickupDateTime { get; set; }
    public int PickupMeterReading { get; set; }
}