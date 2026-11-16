using Inventory.Core.Dtos;

namespace Inventory.Core.Results;

public enum SaveUserStatus
{
    Saved,
    DuplicateEmail
}

public class SaveUserResult
{
    public required SaveUserStatus Status { get; set; }
    public UserResponse? User { get; set; }
}
