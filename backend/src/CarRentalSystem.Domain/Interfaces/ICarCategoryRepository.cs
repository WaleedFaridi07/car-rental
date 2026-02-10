using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Domain.Interfaces;

public interface ICarCategoryRepository
{
    Task<CarCategoryConfig?> GetByIdAsync(int id);
    Task<IEnumerable<CarCategoryConfig>> GetAllAsync();
}
