# Backend conventions

How the API is built and why. For the project layout, see `architecture/architecture.md`.

## Structure

Every resource follows the same path: a **controller** receives the request, a **service** applies the business rules, and a **repository** talks to the database. To add a new resource, copy the Products files and rename.

## Errors

- Expected cases, such as "not found" or "name already exists", are returned by the service as a typed result. The controller turns it into 404 or 409. Exceptions are kept for real failures.
- Error messages for the user are in Spanish and live in `ErrorMessages.cs`. Everything else is in English.
- Every error response carries its message in `title`, so the frontend reads one field.

## Data

- **Nothing is deleted.** Deleting sets `DeletedAt`, and a global filter hides those rows. Unique indexes ignore deleted rows, so a name can be reused.
- Because of that, the database does not stop you from deleting a parent that still has children. The repository checks for them first.
- `CreatedAt` and `UpdatedAt` are filled automatically on save.
- Text inputs are trimmed before they are compared or saved.

## Lists

List endpoints take `page` and `pageSize` and return `items` plus `totalCount` and `totalPages`. It is plain offset pagination because the UI shows numbered pages.

## Code style

- No comments. If code needs one, rewrite it.
- Keep methods short and split the rest into private helpers.
- Create objects with `new X()` and one property per line.

## Before finishing a change

Run `dotnet format`, `dotnet build` and `dotnet test`. Unit tests cover the services in `tests/Inventory.Core.Tests` using in-memory fake repositories. Endpoints (roles, DataAnnotations, pagination limits) are still checked by hand.
