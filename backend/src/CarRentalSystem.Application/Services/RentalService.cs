using CarRentalSystem.Application.Strategies;
using CarRentalSystem.Domain.Dtos;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Repositories;
using CarRentalSystem.Domain.Services;
using CarRentalSystem.Domain.Strategies;

namespace CarRentalSystem.Application.Services;

public class RentalService(
    IRentalRepository rentalRepository,
    ICarCategoryRepository categoryRepository) : IRentalService
{
    private readonly Dictionary<CarCategory, IPricingStrategy> _pricingStrategies = new()
    {
        { CarCategory.SmallCar, new SmallCarPricingStrategy() },
        { CarCategory.Combi, new CombiPricingStrategy() },
        { CarCategory.Truck, new TruckPricingStrategy() }
    };

    public string GenerateBookingNumber()
    {
        var now = DateTime.Now;
        var datePart = now.ToString("yyyyMMdd");
        var timePart = now.ToString("HHmm");
        var random = new Random().Next(100, 1000);
        return $"BK{datePart}{timePart}{random}";
    }

    public async Task<string> RegisterPickupAsync(PickupRegistrationDto dto)
    {
        if (await rentalRepository.BookingNumberExistsAsync(dto.BookingNumber))
        {
            throw new InvalidOperationException("Booking number already exists");
        }

        var rental = new Rental
        {
            BookingNumber = dto.BookingNumber,
            RegistrationNumber = dto.RegistrationNumber,
            CustomerSocialSecurityNumber = dto.CustomerSocialSecurityNumber,
            CarCategoryId = dto.CarCategoryId,
            PickupDateTime = dto.PickupDateTime,
            PickupMeterReading = dto.PickupMeterReading
        };

        await rentalRepository.AddAsync(rental);
        return rental.BookingNumber;
    }

    public async Task<RentalReturnResultDto> RegisterReturnAsync(ReturnRegistrationDto dto)
    {
        var rental = await rentalRepository.GetByBookingNumberAsync(dto.BookingNumber);
        if (rental == null)
        {
            throw new InvalidOperationException("Rental not found");
        }

        if (rental.ReturnDateTime.HasValue)
        {
            throw new InvalidOperationException("Rental already returned");
        }

        if (dto.ReturnMeterReading < rental.PickupMeterReading)
        {
            throw new InvalidOperationException("Return meter reading cannot be less than pickup meter reading");
        }

        var category = await categoryRepository.GetByIdAsync(rental.CarCategoryId);
        if (category == null)
        {
            throw new InvalidOperationException("Car category not found");
        }

        var days = CalculateDays(rental.PickupDateTime, dto.ReturnDateTime);
        var km = dto.ReturnMeterReading - rental.PickupMeterReading;
        var price = CalculatePrice(category.Category, days, km, category.BaseDayRental, category.BaseKmPrice);

        rental.ReturnDateTime = dto.ReturnDateTime;
        rental.ReturnMeterReading = dto.ReturnMeterReading;
        rental.TotalPrice = price;

        await rentalRepository.UpdateAsync(rental);

        return new RentalReturnResultDto
        {
            BookingNumber = rental.BookingNumber,
            DaysRented = days,
            KmDriven = km,
            TotalPrice = price
        };
    }

    public async Task<IEnumerable<Rental>> GetAllRentalsAsync()
    {
        return await rentalRepository.GetAllAsync();
    }

    public async Task<Rental?> GetRentalByBookingNumberAsync(string bookingNumber)
    {
        return await rentalRepository.GetByBookingNumberAsync(bookingNumber);
    }

    public static int CalculateDays(DateTime pickup, DateTime returnDate)
    {
        var timeSpan = returnDate - pickup;
        return (int)Math.Ceiling(timeSpan.TotalDays);
    }

    public decimal CalculatePrice(CarCategory category, int days, int km, decimal baseDayRental, decimal baseKmPrice)
    {
        return !_pricingStrategies.TryGetValue(category, out var strategy) ? throw new InvalidOperationException($"No pricing strategy found for category {category}") : strategy.CalculatePrice(days, km, baseDayRental, baseKmPrice);
    }
}
