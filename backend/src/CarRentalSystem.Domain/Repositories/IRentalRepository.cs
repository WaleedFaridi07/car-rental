using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Domain.Repositories;

public interface IRentalRepository
{
    Task<Rental?> GetByBookingNumberAsync(string bookingNumber);
    Task<IEnumerable<Rental>> GetAllAsync();
    Task<bool> BookingNumberExistsAsync(string bookingNumber);
    Task AddAsync(Rental rental);
    Task UpdateAsync(Rental rental);
}
