# ADR 002: Relational Data Persistence with Entity Framework Core

## Status
Accepted

## Context
Industrial safety systems require strict referential integrity, ACID transactions for equipment reservations, and verifiable audit logging.

## Decision
We implemented Entity Framework Core 8 with a normalized relational schema. Entities inherit from `BaseAuditableEntity`, establishing automated audit metadata (`CreatedAt`, `UpdatedAt`). Enum properties are converted to clean strings in the database to ensure maximum portability.

## Consequences
- Guarantees referential integrity across workers, certificates, permits, and zones.
- Supports both SQLite for fast zero-configuration local development and PostgreSQL for enterprise cloud deployment.
- Enables safe schema recreations, seed migrations, and transactional Lock-Out / Tag-Out equipment bookings.
