using CarRentalSystem.Domain.Dtos;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using FluentResults;

namespace CarRentalSystem.Domain.Services;

public interface IRentalService
{
    Task<Result<string>> RegisterPickupAsync(PickupRegistrationDto dto);
    Task<Result<RentalReturnResultDto>> RegisterReturnAsync(ReturnRegistrationDto dto);
    Task<IEnumerable<Rental>> GetAllRentalsAsync();
    Task<Rental?> GetRentalByBookingNumberAsync(string bookingNumber);
    string GenerateBookingNumber();
    decimal CalculatePrice(CarCategory category, int days, int km, decimal baseDayRental, decimal baseKmPrice);
}