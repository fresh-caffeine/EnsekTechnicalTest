# Ensek Remote Technical Test

## TODO 
- [x] DB - Entity Framework 
  - [x] Add DbContext
  - [x] Add DbSet for entities
  - [x] Configure database connection
  - [x] Add migrations
  - [x] Tables
    - [x] Accounts
    - [x] MeterReadings
- [x] Seed initial data - Test_Accounts.csv
- [ ] API 
  - [ ] POST: meter-reading-uploads (Meter_reading.csv)
    - Question - upload file ~~or plain text~~?
  - [ ] Validate meter reading
    - [ ] Account exists
    - [ ] Reading is a valid number (max 5 digits)
    - [ ] Date is valid
    - [ ] Reading is not a duplicate (or already exists for the same date)
    - [ ] When an account has an existing read, ensure the new read isn’t older than the existing read
    - [ ] Response: 201 Created with the ID of the new reading
    - [ ] Respond with number of records processed, failed and skipped - and row numbers

- [] Final Checks
  - [ ] Add unit tests
  - [ ] Add integration tests
  - [ ] Add documentation
  - [ ] Add logging
  - [ ] Add error handling

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
   This creates MeterReadings.db in your app's root directory
3. If you see the error 'Could not execute because the specified command or file was not found.', 
you may need to install the EF Core tools:
   ```bash
    dotnet tool install --global dotnet-ef
   ```