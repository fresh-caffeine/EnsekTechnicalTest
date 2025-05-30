# Ensek Remote Technical Test - Andy Robinson

## Overview
This project is a .NET Core Web API that allows users to upload meter readings for accounts. It validates the readings against existing data and ensures that they are accurate and not duplicates.
- .NET Core Web API project, using Minimal APIs
- Entity Framework Core for database access
- Uses a SQLite database for simplicity
- Accounts are seeded from a CSV file (`Test_Accounts.csv`) at application startup

## Notes/Assumptions on the implementation:
- AntiForgery is disabled, to simply the API for testing purposes. in a production environment, it would be enabled. Ive added a comment in the code to highlight this.
- Validation - I've assumed that `NNNNN` indicated that 99999 is the maximum reading for an account, I've also assumed that the reading cannot be zero or below.
- I didn't create the client application as part of this submission, but I have provided a Scalar UI for testing the API endpoints. When running the application, you can access Scalar at `http://localhost:5045/scalar/`.
- The Integration tests are pretty naive, I would normally prefer to containerize apis and run integration tests against them, but for the sake of simplicity, I have used the in-memory database for testing purposes.

## Project Structure
- **Ensek.Api**: The main API project.
- **Ensek.Api.Tests**: Contains unit tests for the API.
- **Ensek.Api.IntegrationTests**: Contains integration tests for the API.

## Running the Project
1. Clone the repository.
2. Open the solution in Visual Studio or your preferred IDE.
3. Restore the NuGet packages.
   ```bash
   dotnet restore
   ```
4. Run the application.
   ```bash
    dotnet run
    ```
5. The API will be available at `http://localhost:5045/meter-reading-uploads`.
6. Scalar has been added to the project to test the API endpoints. It's available at http://localhost:5045/scalar/

## Entity Framework 
### Create database with migrations
1. Add a migration:
   ```bash
    dotnet ef migrations add InitialCreate
   ```
2. Update the database:
   ```bash
    dotnet ef database update
   ```
   This creates MeterReadings.db in the app's root directory
3. If you see the error 'Could not execute because the specified command or file was not found.', 
you may need to install the EF Core tools:
   ```bash
    dotnet tool install --global dotnet-ef
   ```
   
## Final Thoughts
Thank you for reviewing my submission. I hope you find the code and documentation clear and easy to follow. I look forward to discussing this in near future.