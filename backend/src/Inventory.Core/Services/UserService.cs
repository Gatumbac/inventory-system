using Inventory.Core.Abstractions;
using Inventory.Core.Constants;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public UserService(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<PagedResult<UserResponse>> GetUsersAsync(UserFilterRequest filterRequest)
    {
        var page = await _users.GetPagedAsync(filterRequest);

        return page.Map(u => ToUserResponse(u));
    }

    public async Task<SaveUserResult> CreateUserAsync(UserRequest userRequest)
    {
        var email = userRequest.Email.Trim().ToLowerInvariant();
        if (await _users.EmailExistsAsync(email))
        {
            return ToSaveResult(SaveUserStatus.DuplicateEmail);
        }

        var user = await ToUser(userRequest, email);
        await _users.AddAsync(user);
        return ToSaveResult(SaveUserStatus.Saved, ToUserResponse(user));
    }

    public async Task<DeleteUserResult> RemoveUserAsync(int id, int currentUserId)
    {
        var user = await _users.GetByIdForUpdateAsync(id);
        if (user == null)
        {
            return DeleteUserResult.NotFound;
        }

        if (user.Id == currentUserId)
        {
            return DeleteUserResult.Self;
        }

        if (user.Role.Name == RoleNames.Admin && await _users.CountAdminsAsync() <= 1)
        {
            return DeleteUserResult.LastAdmin;
        }

        await _users.RemoveAsync(user);
        return DeleteUserResult.Deleted;
    }

    public async Task<ReactivateUserResult> ReactivateUserAsync(int id)
    {
        var user = await _users.GetInactiveByIdForUpdateAsync(id);
        if (user == null)
        {
            return ReactivateUserResult.NotFound;
        }

        if (await _users.EmailExistsAsync(user.Email))
        {
            return ReactivateUserResult.DuplicateEmail;
        }

        await _users.ReactivateAsync(user);
        return ReactivateUserResult.Reactivated;
    }

    private async Task<User> ToUser(UserRequest userRequest, string email)
    {
        var user = new User()
        {
            Name = userRequest.Name.Trim(),
            LastName = userRequest.LastName.Trim(),
            Email = email,
            PasswordHash = _hasher.Hash(userRequest.Password),
            Role = await _users.GetRoleByNameAsync(userRequest.Role)
        };

        return user;
    }

    private static UserResponse ToUserResponse(User user)
    {
        var response = new UserResponse()
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.Name
        };

        return response;
    }

    private static SaveUserResult ToSaveResult(SaveUserStatus status, UserResponse? user = null)
    {
        var result = new SaveUserResult()
        {
            Status = status,
            User = user
        };

        return result;
    }
}
