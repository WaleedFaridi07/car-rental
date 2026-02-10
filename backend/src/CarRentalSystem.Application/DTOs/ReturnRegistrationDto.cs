namespace CarRentalSystem.Application.DTOs;

public class ReturnRegistrationDto
{
    public string BookingNumber { get; set; } = string.Empty;
    public DateTime ReturnDateTime { get; set; }
    public int ReturnMeterReading { get; set; }
}
