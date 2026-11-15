using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Dtos;

public class SupplierRequest
{
    [Required]
    [StringLength(150)]
    public required string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
