using System.ComponentModel.DataAnnotations;
using Inventory.Core.Constants;

namespace Inventory.Core.Dtos;

public class UserRequest
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    [StringLength(100)]
    public required string LastName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = ErrorMessages.UserEmailInvalid)]
    [StringLength(255)]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = ErrorMessages.UserPasswordInvalid)]
    public required string Password { get; set; }

    [Required]
    [AllowedValues(RoleNames.Admin, RoleNames.Viewer, ErrorMessage = ErrorMessages.UserRoleInvalid)]
    public required string Role { get; set; }
}
