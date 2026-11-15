using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Dtos;

public class ProductRequest : IValidatableObject
{
    [Required]
    [StringLength(200)]
    public required string Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public List<ProductAttributeRequest> Attributes { get; set; } = new List<ProductAttributeRequest>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var uniqueCount = Attributes.Select(a => a.Name.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        if (uniqueCount != Attributes.Count)
        {
            var fields = new List<string> { nameof(Attributes) };
            results.Add(new ValidationResult("Attribute names must be unique.", fields));
        }

        return results;
    }
}
