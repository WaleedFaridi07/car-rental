using CarRentalSystem.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICarCategoryRepository categoryRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await categoryRepository.GetAllAsync();
        return Ok(categories);
    }
}
