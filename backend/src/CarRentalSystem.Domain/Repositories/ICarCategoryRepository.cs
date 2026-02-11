using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Domain.Repositories;

public interface ICarCategoryRepository
{
    Task<CarCategoryConfig?> GetByIdAsync(int id);
    Task<IEnumerable<CarCategoryConfig>> GetAllAsync();
}
