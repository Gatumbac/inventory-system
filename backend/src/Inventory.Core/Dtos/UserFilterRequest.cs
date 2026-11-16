using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Dtos;

public class UserFilterRequest : PageRequest
{
    [StringLength(100)]
    public string? Search { get; set; }
}
