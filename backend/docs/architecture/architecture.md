# Backend architecture

The backend follows Clean Architecture, split into three projects.

## Projects

- **Inventory.Core**: entities, interfaces, DTOs and constants. It has no dependencies.
- **Inventory.Infrastructure**: implements the Core interfaces (database access, password hashing, tokens). References Core.
- **Inventory.Api**: controllers and `Program.cs`. It only returns JSON, the real UI is the Angular app. References Core and Infrastructure.

## Dependencies

```
Api --> Core <-- Infrastructure
```

Dependencies always point toward Core. Infrastructure never references Api, it would be a circular reference.

## Rules

- Controllers only use Core interfaces, DTOs and constants. They never touch the database context directly.
- `Program.cs` is the only place that knows the concrete implementations and registers them.
- An interface lives in Core, its implementation lives in Infrastructure. For example, the password hasher interface is in Core and the implementation is in Infrastructure.
