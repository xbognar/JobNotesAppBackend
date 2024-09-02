
# Job Notes App API Documentation

The Job Notes App API is designed to manage job notes, including tasks, clients, and locations. The backend is developed using C# and .NET, with an MSSQL database. The project is designed to run locally, utilizing Docker containers for both the database and backend services. This setup ensures efficient management of job-related data and provides secure user authentication via JWT (JSON Web Tokens). The API is rigorously tested using unit tests for services and controllers, ensuring reliability and correctness. The containerized architecture using Docker and Docker Compose ensures easy deployment and a consistent runtime environment.

## Technologies Used

- **Programming Language:** C#
- **Framework:** .NET 8.0
- **Database:** MSSQL
- **ORM:** Entity Framework Core
- **Authentication:** JWT (JSON Web Tokens)
- **Containerization:** Docker
- **API Documentation:** Swagger
- **Testing:** xUnit, Moq

## Features

- **User Authentication:** Secure login with JWT tokens.
- **Job Management:** CRUD operations for jobs.
- **Job Search:** Ability to search jobs by location, client name, or notes.
- **Job Statistics:** Retrieve job counts by year and month.
- **Global Error Handling:** Centralized handling of exceptions.
- **Integration Tests:** Ensuring the API endpoints function correctly.
- **Unit Tests:** Testing the core logic and services.
- **Automated Scripts:** Simplified start and stop scripts for running the application.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (Must be installed and running on the user's PC)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Project Structure

```
JobNotesAppBackend/
├── src/
│   ├── JobNotesAPI/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   └── JobsController.cs
│   │   ├── Program.cs
│   │   ├── JobNotesAPI.csproj
│   │   ├── appsettings.json
│   │   └── ...
│   ├── DataAccess/
│   │   ├── DataAccess/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   └── IJobService.cs
│   │   ├── Models/
│   │   │   ├── Job.cs
│   │   │   └── User.cs
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   └── JobService.cs
│   │   └── DataAccess.csproj
├── tests/
│   ├── AuthControllerTests/
│   │   ├── AuthControllerTests.cs
│   ├── JobServiceTests/
│   │   ├── JobServiceTests.cs
├── Dockerfile
├── docker-compose.yml
├── .env
├── README.md
├── START.bat
└── STOP.bat
```

### Script Details

- **START.bat:** This script starts Docker Desktop (if not already running), navigates to the project directory, and starts the Docker containers using Docker Compose. It allows the user to run the application with a single click.
- **STOP.bat:** This script stops all running Docker containers related to the application and stops Docker Desktop. It provides a clean and easy way to shut down the application.

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/yourusername/job-notes-app-api.git
   cd job-notes-app-api
   ```

2. **Create a `.env` file in the root directory:**

   ```bash
   SA_PASSWORD=YourStrong@Passw0rd
   CONNECTION_STRING="Server=mssql;Database=JobNotesDB;User Id=sa;Password=YourStrong@Passw0rd;"
   AUTH_USERNAME=your_auth_username
   AUTH_PASSWORD=your_auth_password
   JWT_KEY=your_very_secure_jwt_key
   ```

3. **Run the application using the START script:**

   Simply double-click the `START.bat` file. This will automatically start Docker Desktop (if not running), build and run the containers, and set up the application environment.

4. **Apply migrations:**

   Migrations will be applied automatically when the application starts.

## Usage

1. **Starting the API:**

   The API will be available at `http://localhost:5000` after running the `START.bat` script.

2. **Stopping the API:**

   To stop the API and Docker containers, double-click the `STOP.bat` file. This will shut down the containers and stop Docker Desktop.

3. **Accessing Swagger:**

   Swagger documentation will be available at `http://localhost:5000/swagger`.

4. **Endpoints:**

   - **Authentication:**
     - `POST /api/auth/login`

   - **Jobs:**
     - `GET /api/jobs`
     - `GET /api/jobs/{id}`
     - `POST /api/jobs`
     - `PUT /api/jobs/{id}`
     - `DELETE /api/jobs/{id}`
     - `GET /api/jobs/count/year/{year}`
     - `GET /api/jobs/count/year/{year}/month/{month}`
     - `GET /api/jobs/search?location=example&clientName=example&notes=example`

## JWT Authentication

The application uses JWT (JSON Web Tokens) for secure authentication. Users must log in using their credentials to receive a JWT token, which must be included in the Authorization header (`Bearer <token>`) for all subsequent requests. The token is valid for 2 hours, ensuring a balance between security and usability.

## Testing

### Run Unit and Integration Tests:

Run the following commands to execute the tests:

```bash
dotnet test tests/AuthControllerTests/
dotnet test tests/JobServiceTests/
```

- **Mocking:** The tests use Moq for mocking dependencies, ensuring that the controllers and services are tested in isolation.
- **Endpoint Testing:** Each endpoint is tested to confirm it handles both valid and invalid inputs correctly, verifying authentication, authorization, and business logic.

