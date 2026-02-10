using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Interfaces;
using CarRentalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Repositories;

public class CarCategoryRepository(CarRentalDbContext context) : ICarCategoryRepository
{
    public async Task<CarCategoryConfig?> GetByIdAsync(int id)
    {
        return await context.CarCategoryConfigs.FindAsync(id);
    }

    public async Task<IEnumerable<CarCategoryConfig>> GetAllAsync()
    {
        return await context.CarCategoryConfigs.ToListAsync();
    }
}
