namespace CarRentalSystem.Application.DTOs;

public class RentalReturnResult
{
    public string BookingNumber { get; set; } = string.Empty;
    public int DaysRented { get; set; }
    public int KmDriven { get; set; }
    public decimal TotalPrice { get; set; }
}
