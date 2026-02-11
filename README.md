# 🚗 Car Rental (React + .NET 10 + PostgreSQL)

A small **car rental portal** with a minimal **React UI** and a **.NET 10 Web API** backend, backed by **PostgreSQL** (Docker).  
The goal is to implement and verify core rental business rules (pickup/return + price calculation) in a clean, testable way.

---

## ✨ What’s included

- **Backend (.NET 10 Web API)**
  - Register **car pickup**
  - Register **car return**
  - Calculate rental price based on car category rules
  - Persistence in **PostgreSQL**
  - Swagger/OpenAPI for easy exploration
  - Unit + (optional) integration tests

- **Frontend (React)**
  - Minimal UI to call backend endpoints
  - Simple flows for pickup/return
  - Lightweight setup (typical Vite-based React app)

---

## 📦 Tech stack

**Backend**
- .NET 10
- ASP.NET Core Web API
- EF Core + Npgsql (PostgreSQL provider)
- xUnit (tests)
- (Optional) Testcontainers for PostgreSQL integration tests

**Frontend**
- React + TypeScript
- Vite (dev server + build tooling)

**Database**
- PostgreSQL (Docker)

---

## 🧠 Business rules (pricing)

Rental price is calculated using two configurable inputs:
- `baseDayRental`
- `baseKmPrice`

Categories:

- **Small car**  
  `Price = baseDayRental * numberOfDays`

- **Combi**  
  `Price = baseDayRental * numberOfDays * 1.3 + baseKmPrice * numberOfKm`

- **Truck**  
  `Price = baseDayRental * numberOfDays * 1.5 + baseKmPrice * numberOfKm * 1.5`

> The design assumes more categories may be added later (e.g., new pricing strategies).

---

## ✅ Use cases implemented

### 1) Register car pickup
When the rental company hands out a car:

- Booking number (unique)
- Registration number
- Customer SSN
- Car category
- Pick-up date/time
- Current odometer (km)

### 2) Register returned car
When the customer returns a car:

- Booking number
- Return date/time
- Current odometer (km)

After return registration, the system calculates the total price using the formulas above.

---

## 🔎 Assumptions (explicit)
Because the original spec leaves some details open, this project uses these assumptions:

1. **Billable days**
   - `numberOfDays = max(1, ceil((returnTime - pickupTime).TotalDays))`
2. **Driven kilometers**
   - `numberOfKm = returnOdometer - pickupOdometer`
   - return odometer must be **>=** pickup odometer
3. **Timestamps**
   - Stored as `DateTimeOffset` (recommended for timezone safety)
4. **Base prices**
   - `baseDayRental` and `baseKmPrice` are configurable (appsettings/env)

If you want different billing rules (e.g., charge per started calendar day, or hourly), update the calculation logic + tests accordingly.

---

## 🗂️ Repository structure

```text
.
├── backend/     # .NET 10 API + domain logic + tests
└── frontend/    # React UI (minimal portal)


⸻

🚀 Getting started

Prerequisites
	•	.NET 10 SDK
	•	Node.js (LTS recommended)
	•	Docker (for PostgreSQL)

⸻

🐘 Start PostgreSQL with Docker

From repo root (or wherever your compose file lives):

docker compose up -d

If you don’t have a docker-compose.yml yet, add one that exposes Postgres on 5432 and sets user/password/db name.

⸻

🔧 Run the backend

cd backend
dotnet restore
dotnet run

Then open Swagger (adjust port if needed):
	•	https://localhost:<port>/swagger

⸻

🖥️ Run the frontend

cd frontend
npm install
npm run dev

By default Vite runs on:
	•	http://localhost:5173

Make sure the frontend is configured to call the backend base URL (via .env or a config file).

⸻

🧪 Run tests

cd backend
dotnet test

Integration tests (optional)

If the backend includes Testcontainers-based integration tests:
	•	ensure Docker is running
	•	run dotnet test

If you see this warning in tests:
CS0618: PostgreSqlBuilder() is obsolete
use the new constructor: new PostgreSqlBuilder("postgres:16-alpine")

⸻

🔌 API overview (example)

Endpoints may differ slightly depending on your implementation—update this section to match.

Register pickup

POST /rentals/pickup

Example payload:

{
  "bookingNumber": "B-10001",
  "registrationNumber": "ABC123",
  "customerSsn": "YYYYMMDD-XXXX",
  "carCategory": "SmallCar",
  "pickupTime": "2026-02-10T10:00:00+01:00",
  "pickupOdometerKm": 12500
}

Register return

POST /rentals/return

{
  "bookingNumber": "B-10001",
  "returnTime": "2026-02-12T13:30:00+01:00",
  "returnOdometerKm": 12740
}

Expected result:
	•	persisted return details
	•	calculated price
	•	derived numberOfDays + numberOfKm

⸻

🧱 Design notes
	•	Business logic is kept testable and independent from UI + database concerns.
	•	Category pricing is implemented in a way that supports adding new categories later (e.g., via strategy pattern or mapping to pricing rules).
	•	EF Core handles persistence; Postgres is used for local dev via Docker.

⸻

🗺️ Roadmap / nice-to-haves
	•	Authentication (JWT / OAuth)
	•	Validation & error handling (FluentValidation)
	•	Database migrations + seed data
	•	Better UI flows (history, details, status)
	•	More categories + promotions/discount rules
	•	CI pipeline (build + test + lint)
