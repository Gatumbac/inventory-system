using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Dtos;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
