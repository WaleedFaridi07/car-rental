using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Dtos;
using CarRentalSystem.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController(IRentalService rentalService) : ControllerBase
{
    [HttpPost("pickup")]
    public async Task<IActionResult> RegisterPickup([FromBody] PickupRegistrationDto dto)
    {
        try
        {
            var bookingNumber = await rentalService.RegisterPickupAsync(dto);
            return Ok(bookingNumber);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("return")]
    public async Task<IActionResult> RegisterReturn([FromBody] ReturnRegistrationDto dto)
    {
        try
        {
            var result = await rentalService.RegisterReturnAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRentals()
    {
        var rentals = await rentalService.GetAllRentalsAsync();
        return Ok(rentals);
    }

    [HttpGet("{bookingNumber}")]
    public async Task<IActionResult> GetRentalByBookingNumber(string bookingNumber)
    {
        var rental = await rentalService.GetRentalByBookingNumberAsync(bookingNumber);
        if (rental == null)
        {
            return NotFound(new { error = "Rental not found" });
        }
        return Ok(rental);
    }

    [HttpGet("generate-booking-number")]
    public IActionResult GenerateBookingNumber()
    {
        var bookingNumber = rentalService.GenerateBookingNumber();
        return Ok(new { bookingNumber });
    }
}
