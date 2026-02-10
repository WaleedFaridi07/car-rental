# Car Rental Management System

A full-stack car rental management system built with .NET 10 REST API backend and React TypeScript frontend.

## Features

### Business Features
- **Three Car Categories** with different pricing formulas:
  - Small Car: 300 SEK/day (base × days)
  - Combi: 500 SEK/day + 2 SEK/km (base × days × 1.3 + km × price)
  - Truck: 800 SEK/day + 3 SEK/km (base × days × 1.5 + km × price × 1.5)

- **Register Pickup**: Auto-generate unique booking numbers, capture vehicle and customer details
- **Register Return**: Calculate rental costs, validate meter readings, prevent duplicate returns
- **View All Rentals**: List all rentals with status, quick return action for active rentals

### Technical Features
- Clean Architecture with separation of concerns
- Strategy Pattern for pricing calculations
- Repository Pattern for data access
- PostgreSQL database with Entity Framework Core
- Comprehensive unit tests (17+ tests)
- RESTful API with Swagger documentation
- Responsive React UI with Tailwind CSS
- Real-time data updates with React Query

## Architecture

```
CarRentalSystem/
├── src/
│   ├── CarRentalSystem.API/          # REST API Controllers
│   ├── CarRentalSystem.Application/  # Business Logic & Services
│   ├── CarRentalSystem.Domain/       # Entities & Interfaces
│   └── CarRentalSystem.Infrastructure/ # Data Access & Repositories
├── tests/
│   ├── CarRentalSystem.UnitTests/    # Unit Tests
│   └── CarRentalSystem.IntegrationTests/ # Integration Tests
└── docker-compose.yml                # PostgreSQL Container
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://www.docker.com/) (for PostgreSQL)
- [PostgreSQL](https://www.postgresql.org/) (or use Docker)

## Setup Instructions

### 1. Start PostgreSQL Database

```bash
cd CarRentalSystem
docker-compose up -d
```

This starts PostgreSQL on port 5432 with:
- Database: `carrental`
- Username: `carrental`
- Password: `carrental123`

### 2. Setup Backend

```bash
# Navigate to the solution directory
cd CarRentalSystem

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update --project src/CarRentalSystem.Infrastructure/CarRentalSystem.Infrastructure.csproj --startup-project src/CarRentalSystem.API/CarRentalSystem.API.csproj

# Run the API
dotnet run --project src/CarRentalSystem.API/CarRentalSystem.API.csproj
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger`

### 3. Setup Frontend

```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Start development server
npm run dev
```

The frontend will be available at: `http://localhost:5173`

## Running Tests

```bash
# Run all tests
cd CarRentalSystem
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## API Endpoints

### Rentals
- `POST /api/rentals/pickup` - Register a new pickup
- `POST /api/rentals/return` - Register a return
- `GET /api/rentals` - Get all rentals
- `GET /api/rentals/{bookingNumber}` - Get rental by booking number
- `GET /api/rentals/generate-booking-number` - Generate new booking number

### Categories
- `GET /api/categories` - Get all car categories

## Database Schema

### Rentals Table
- `BookingNumber` (PK) - Unique booking identifier
- `RegistrationNumber` - Vehicle registration
- `CustomerSocialSecurityNumber` - Customer SSN
- `CarCategoryId` (FK) - Reference to car category
- `PickupDateTime` - Pickup date and time
- `PickupMeterReading` - Odometer reading at pickup
- `ReturnDateTime` - Return date and time (nullable)
- `ReturnMeterReading` - Odometer reading at return (nullable)
- `TotalPrice` - Calculated rental price (nullable)

### CarCategoryConfigs Table
- `Id` (PK) - Category identifier
- `Category` - Enum (SmallCar=1, Combi=2, Truck=3)
- `BaseDayRental` - Base daily rental price
- `BaseKmPrice` - Base price per kilometer

## Design Patterns

### Strategy Pattern
Used for pricing calculations. Each car category has its own pricing strategy:
- `SmallCarPricingStrategy`
- `CombiPricingStrategy`
- `TruckPricingStrategy`

### Repository Pattern
Abstracts data access logic:
- `IRentalRepository` / `RentalRepository`
- `ICarCategoryRepository` / `CarCategoryRepository`

### Clean Architecture
Separates concerns into layers:
- **Domain**: Core business entities and interfaces
- **Application**: Business logic and services
- **Infrastructure**: Data access and external dependencies
- **API**: HTTP endpoints and controllers

## Frontend Structure

```
frontend/src/
├── api/
│   └── client.ts           # API client with axios
├── components/
│   └── Layout.tsx          # Main layout with sidebar
├── pages/
│   ├── RegisterPickup.tsx  # Pickup registration form
│   ├── RegisterReturn.tsx  # Return registration form
│   └── AllRentals.tsx      # Rentals list table
├── types/
│   └── index.ts            # TypeScript interfaces
├── App.tsx                 # Main app component
└── main.tsx                # Entry point
```

## Technologies Used

### Backend
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- PostgreSQL (Npgsql)
- Swashbuckle (Swagger)
- xUnit, Moq, FluentAssertions (Testing)

### Frontend
- React 18
- TypeScript
- Vite
- React Router
- React Query (TanStack Query)
- React Hook Form
- Axios
- Tailwind CSS
- React Hot Toast

## Validation Rules

- Booking numbers must be unique
- Meter reading on return must be >= pickup reading
- Cannot return the same rental twice
- All required fields must be provided
- Dates must be valid

## Pricing Formulas

### Small Car
```
Price = BaseDayRental × NumberOfDays
Example: 300 × 3 = 900 SEK
```

### Combi
```
Price = (BaseDayRental × NumberOfDays × 1.3) + (BaseKmPrice × NumberOfKm)
Example: (500 × 2 × 1.3) + (2 × 100) = 1300 + 200 = 1500 SEK
```

### Truck
```
Price = (BaseDayRental × NumberOfDays × 1.5) + (BaseKmPrice × NumberOfKm × 1.5)
Example: (800 × 3 × 1.5) + (3 × 150 × 1.5) = 3600 + 675 = 4275 SEK
```

## Development

### Adding a New Migration

```bash
dotnet ef migrations add MigrationName --project src/CarRentalSystem.Infrastructure/CarRentalSystem.Infrastructure.csproj --startup-project src/CarRentalSystem.API/CarRentalSystem.API.csproj
```

### Building for Production

```bash
# Backend
dotnet publish -c Release

# Frontend
cd frontend
npm run build
```

## Troubleshooting

### Database Connection Issues
- Ensure PostgreSQL is running: `docker ps`
- Check connection string in `appsettings.json`
- Verify database exists: `docker exec -it carrental-postgres psql -U carrental -d carrental`

### Port Conflicts
- Backend uses ports 5000 (HTTP) and 5001 (HTTPS)
- Frontend uses port 5173
- PostgreSQL uses port 5432

### CORS Issues
- Ensure the API CORS policy includes your frontend URL
- Check browser console for CORS errors

## License

This project is for educational purposes.
