using Inventory.Core.Dtos;
using Inventory.Core.Results;

namespace Inventory.Core.Abstractions;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetUsersAsync(UserFilterRequest filterRequest);
    Task<SaveUserResult> CreateUserAsync(UserRequest userRequest);
    Task<DeleteUserResult> RemoveUserAsync(int id, int currentUserId);
    Task<ReactivateUserResult> ReactivateUserAsync(int id);
}
