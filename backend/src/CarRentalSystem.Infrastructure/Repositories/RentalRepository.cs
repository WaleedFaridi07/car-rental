using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Repositories;

public class RentalRepository(CarRentalDbContext context) : IRentalRepository
{
    public async Task<Rental?> GetByBookingNumberAsync(string bookingNumber)
    {
        return await context.Rentals
            .Include(r => r.CarCategory)
            .FirstOrDefaultAsync(r => r.BookingNumber == bookingNumber);
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        return await context.Rentals
            .Include(r => r.CarCategory)
            .OrderByDescending(r => r.PickupDateTime)
            .ToListAsync();
    }

    public async Task<bool> BookingNumberExistsAsync(string bookingNumber)
    {
        return await context.Rentals.AnyAsync(r => r.BookingNumber == bookingNumber);
    }

    public async Task AddAsync(Rental rental)
    {
        await context.Rentals.AddAsync(rental);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Rental rental)
    {
        context.Rentals.Update(rental);
        await context.SaveChangesAsync();
    }
}
