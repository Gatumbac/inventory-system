using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Dtos;

public class ProductAttributeRequest
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    [StringLength(200)]
    public required string Value { get; set; }

}
