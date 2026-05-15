# InvoiceGenerator

## Overview
InvoiceGenerator is a .NET Worker Service solution that automates invoice generation by pulling orders from a database, creating invoices through the XL API, and updating invoice attributes. The service runs on a schedule and is designed to operate as a Windows service.

## Project Structure
- **InvoiceGenerator.Service**: Worker service responsible for coordinating background invoice generation workflows.
- **InvoiceGenerator.Infrastructure**: Infrastructure layer that handles database access, external integrations (XL API), and implementation details.
- **InvoiceGenerator.Contracts**: Shared contracts and models used across the solution.

## Key Features
- Scheduled background invoice generation via a .NET Worker Service.
- Integration with the XL API for invoice creation and error reporting.
- Dapper-based SQL access for order retrieval and invoice metadata lookups.
- Attribute updates on generated invoices (e.g., StatusWMS).
- Structured logging with Serilog.

## Runtime Workflow
1. The service starts, logs in to the XL API, and waits for the configured schedule window.
2. Orders are fetched from the database and grouped with line items.
3. Each order is converted into an invoice through the XL API, with optional attribute updates.
4. Results and failures are logged, and the cycle repeats on the configured interval.

## Data and Integrations
- **Database**: SQL Server access through Dapper and stored procedures.
- **External API**: XL API for invoice creation, transaction management, and error descriptions.

## Logging and Operations
- Runs as a Windows service with graceful startup/shutdown hooks.
- Logs operational and error information using Serilog.

## Technology Stack
- .NET 10
- Dapper
- Serilog

## License

This project is **proprietary and confidential**.

It was developed for a client and is **not permitted to be shared, redistributed, or used** without explicit written permission from the owner.

See [LICENSE](LICENSE) for details.

---

© 2026-present [calKU0](https://github.com/calKU0)
