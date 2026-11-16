# Validation and business rules

There are three layers, and each one fails with a different status:

| Layer | Where | Status | Example in the repo |
| --- | --- | --- | --- |
| Shape of the input | DataAnnotations on the Request DTO | 400 (automatic, `[ApiController]`) | `[StringLength(150)]` in `SupplierRequest`, `[RegularExpression]` on `ProductRequest.Code` |
| Rules across fields of one request | `IValidatableObject.Validate` on the Request DTO | 400 | Unique attribute names in `ProductRequest` |
| Rules that need the database | Service → typed result → controller | 404 / 409 (or 422) | `SaveSupplierStatus.DuplicateName` → 409 |
| Last line of defense | Unique index / check constraint in the EF configuration | 500 if reached | `CK_ProductSuppliers_Price_NonNegative` |

Every user-facing message is a constant in `Core/Constants/ErrorMessages.cs`, in Spanish.

## A. Field rule (400)

```csharp
[Required]
[StringLength(20, MinimumLength = 3)]
[RegularExpression("^[0-9+ -]+$", ErrorMessage = ErrorMessages.PhoneInvalid)]
public string? Phone { get; set; }

[Range(typeof(decimal), "0.01", "999999", ErrorMessage = ErrorMessages.PriceInvalid)]
public decimal Price { get; set; }
```

The 400 body has `errors: { "Phone": ["…"] }`. The frontend shows the first message (`error.interceptor.ts`).

## B. Rule across fields of the same request (400)

**When:** "The minimum stock cannot be greater than the maximum", "the end date must be after the start date".

```csharp
public class ProductRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        if (MinStock > MaxStock)
        {
            var fields = new List<string>()
            {
                nameof(MinStock)
            };
            results.Add(new ValidationResult(ErrorMessages.MinStockGreaterThanMax, fields));
        }

        return results;
    }
}
```

`ProductRequest` already implements it, so add the new check inside the existing `Validate`.

## C. Rule that needs the database (typed result)

**When:** "Don't allow two products with the same name", "can't delete a supplier with stock", "a lot's stock can't go below zero after a sale".

1. Add a status value:

```csharp
public enum SaveProductStatus
{
    Saved,
    NotFound,
    DuplicateCode,
    DuplicateName
}
```

2. Add the repository check (interface + EF + fake):

```csharp
Task<bool> NameExistsAsync(string name, int? excludeId = null);
```

```csharp
public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
{
    return await _context.Products.AnyAsync(p => p.Name == name && p.Id != excludeId);
}
```

`excludeId` lets the product being updated keep its own name.

3. Check it in the service, before saving:

```csharp
if (await _products.NameExistsAsync(name, id))
{
    return ToSaveResult(SaveProductStatus.DuplicateName);
}
```

4. Add the message and the mapping in the controller:

```csharp
public const string ProductNameDuplicated = "Ya existe un producto con ese nombre";
```

```csharp
if (result.Status == SaveProductStatus.DuplicateName)
{
    return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.ProductNameDuplicated);
}
```

Also add `[ProducesResponseType(StatusCodes.Status409Conflict)]` if it is new for that action.

5. Test it ([testing.md](testing.md)).

Which status code:

| Case | Code |
| --- | --- |
| Resource in the URL does not exist | 404 |
| Duplicate / still has children / state conflict | 409 |
| Body is well-formed but breaks a business rule (stock would go negative) | 422 `StatusCodes.Status422UnprocessableEntity` |

More real examples in `UserService`: `DeleteUserResult.Self` (cannot deactivate yourself) and `DeleteUserResult.LastAdmin` (cannot remove the last admin), both mapped to 409 in `UserController`.

## D. Database guarantee

Back the rule with the database when two requests at the same time could both pass the check:

```csharp
builder.HasIndex(p => p.Name).IsUnique().HasFilter("[DeletedAt] IS NULL");
builder.ToTable(t => t.HasCheckConstraint("CK_Products_MinStock_NonNegative", "[MinStock] >= 0"));
```

The filter `[DeletedAt] IS NULL` lets a soft-deleted name be reused. This needs a migration, and the migration fails if the existing data already breaks the rule.

## E. Normalization

The services trim text before comparing and saving (`NormalizeName`, `NormalizeCode` → upper-case). Do the same for new text fields, so `" Acme"` and `"Acme"` count as duplicates.

## Verify

- Swagger: break each rule once, check the status code and that `title` (or `errors`) has the Spanish message.
- A unit test per rule.

Next: [frontend/forms-and-validation.md](../frontend/forms-and-validation.md).
