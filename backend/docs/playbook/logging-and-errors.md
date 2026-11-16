# Logging and errors

## Logging

Serilog is configured from `appsettings.json` (`builder.Services.AddSerilog(... ReadFrom.Configuration ...)`):

- Sinks: console and a daily file `logs/log-.txt` (relative to where the API runs, so `backend/logs/` on the host).
- `UseSerilogRequestLogging()` writes one line per HTTP request with method, path, status and time.
- Levels: `Information` by default, `Warning` for ASP.NET Core and for EF SQL commands.

### Log from your code

Inject `ILogger<T>` and use message templates (named placeholders, not string interpolation):

```csharp
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }
}
```

```csharp
_logger.LogInformation("Product {ProductId} deleted by user {UserId}", id, userId);
_logger.LogWarning("Stock for product {ProductId} is below {Threshold}", id, threshold);
_logger.LogError(exception, "Price import failed for supplier {SupplierId}", supplierId);
```

`ILogger<T>` lives in `Microsoft.Extensions.Logging`. `Core` services can use it too: add the `Microsoft.Extensions.Logging.Abstractions` package to `Inventory.Core` (ask first, it is a new dependency). Otherwise, log in the controller.

Never log passwords, tokens or full request bodies.

### See SQL

Change `"Microsoft.EntityFrameworkCore.Database.Command": "Warning"` to `"Information"` in `appsettings.json` while you test, then change it back.

## Errors

| Case | Who handles it | Response |
| --- | --- | --- |
| Invalid input (DataAnnotations, `IValidatableObject`) | `[ApiController]` | 400 `ValidationProblemDetails` with `errors` |
| Expected outcome (not found, duplicate, has children) | Service status → controller | `Problem(statusCode: 404/409, title: ErrorMessages.X)` |
| No token / wrong role | Authentication / authorization | 401 / 403 (empty body, `UseStatusCodePages` adds a ProblemDetails) |
| Unexpected exception | `Api/Middleware/GlobalExceptionHandler.cs` | Logged with method and path, 500 with `ErrorMessages.UnexpectedError` |

The frontend reads `title` for every error and `errors` for 400 (`frontend/src/app/core/http/error.interceptor.ts`).

### New expected error

1. Constant in `Core/Constants/ErrorMessages.cs`, in Spanish.
2. Status value in the `Results` enum.
3. Return it from the service.
4. Map it in the controller with `Problem(...)`, and add `[ProducesResponseType]`.

Do not throw exceptions for expected cases, and do not catch exceptions in controllers. The global handler already logs and answers.

### Handle a specific DB error

**When:** two requests at the same time both pass the duplicate check and the unique index rejects the second one. The current code returns a 500 for it. If asked to turn it into a 409:

```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
{
    return false;
}
```

2601/2627 are SQL Server's duplicate-key errors. `SqlException` comes from `Microsoft.Data.SqlClient`. Do this in the repository, return a bool or a status, and keep `Core` free of EF types.
