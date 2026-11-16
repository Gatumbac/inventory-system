# Authentication and roles

## How it works today

- `POST /api/auth/login` → `AuthService` checks the password hash → `JwtTokenService.GenerateToken` signs a JWT with claims `sub` (user id), `email` and `role`. The lifetime comes from `JwtSettings.ExpirationMinutes` (60 by default).
- `Program.cs` validates issuer, audience, lifetime and signature. `NameClaimType = "sub"`, `RoleClaimType = "role"`, `MapInboundClaims = false`, so claim names stay short.
- Roles: `RoleNames.Admin` and `RoleNames.Viewer` (`Core/Constants/RoleNames.cs`), stored in the `Roles` table through `HasData` in `RoleConfiguration`.
- Controllers: `[Authorize]` on the class (any logged-in user), `[Authorize(Roles = RoleNames.Admin)]` on writes.
- No token → 401. Wrong role → 403.

## A. Change who can do what

```csharp
[Authorize(Roles = RoleNames.Admin)]
```

Several roles (comma = OR):

```csharp
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.Editor)]
```

Public endpoint (no login):

```csharp
[AllowAnonymous]
```

Always change the frontend too: hide the button and guard the route ([frontend/roles-in-ui.md](../frontend/roles-in-ui.md)).

## B. Add a role

Example: `Editor` (can create and edit, cannot delete).

1. `Core/Constants/RoleNames.cs`:

```csharp
public const string Editor = "Editor";
```

2. `Infrastructure/Persistence/Configurations/RoleConfiguration.cs`, a third `HasData` row:

```csharp
var editorRole = new Role()
{
    Id = 3,
    Name = RoleNames.Editor,
    Description = "Puede crear y editar, no eliminar"
};

builder.HasData(adminRole, viewerRole, editorRole);
```

3. Migration `AddEditorRole` ([migrations.md](migrations.md)).

4. A demo user: in `DbSeeder.Seed`, add `EnsureUser(context, hasher, "Editor", "System", editorEmail, editorPassword, RoleNames.Editor);` with two new settings `Seed:EditorEmail` / `Seed:EditorPassword` read with `GetSetting`. Add `Seed__EditorEmail` and `Seed__EditorPassword` to `.env` and `.env.example`. `GetSetting` throws if a setting is missing, so the API will not start without them.

5. Controllers: POST/PUT → `Admin,Editor`; DELETE stays `Admin`.

6. Frontend: `AuthService.isAdmin()` only knows Admin. Add a `canEdit()` that accepts both roles, and use it for buttons and the guard.

## C. Who is the current user

In a controller (the same as `AuthController.Me`):

```csharp
var userId = int.Parse(User.FindFirst("sub")!.Value);
var role = User.FindFirst("role")!.Value;
var isAdmin = User.IsInRole(RoleNames.Admin);
```

Pass `userId` to the service as a parameter. `Core` does not read HTTP. A real example: `UserController.RemoveUser` reads `sub` and passes it to `UserService.RemoveUserAsync(id, currentUserId)`, which returns `DeleteUserResult.Self` so an admin cannot deactivate themselves.

`UserController` is also the example of a whole controller restricted to admins: `[Authorize(Roles = RoleNames.Admin)]` on the class.

**When:** "Record who created the product". Add `CreatedByUserId` (int?) to the entity, a FK to `User` ([relationships.md](relationships.md)), and set it from the controller → service.

## D. Token lifetime

`JwtSettings.ExpirationMinutes` defaults to 60. To change it, add `Jwt__ExpirationMinutes` to `.env` (it is not there today). There is no refresh token. When it expires, the frontend gets a 401 and goes back to login.

## Verify

- Swagger: log in with each demo user, copy the token, use the **Authorize** button, call the endpoint. Expect 200/201 or 403.
- Without a token → 401.
