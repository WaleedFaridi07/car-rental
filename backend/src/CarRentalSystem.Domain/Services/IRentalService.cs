using CarRentalSystem.Domain.Dtos;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Services;

public interface IRentalService
{
    Task<string> RegisterPickupAsync(PickupRegistrationDto dto);
    Task<RentalReturnResultDto> RegisterReturnAsync(ReturnRegistrationDto dto);
    Task<IEnumerable<Rental>> GetAllRentalsAsync();
    Task<Rental?> GetRentalByBookingNumberAsync(string bookingNumber);
    string GenerateBookingNumber();
    decimal CalculatePrice(CarCategory category, int days, int km, decimal baseDayRental, decimal baseKmPrice);
}