using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Dtos;

public class ProductFilterRequest : PageRequest
{
    [StringLength(100)]
    public string? Search { get; set; }
}
