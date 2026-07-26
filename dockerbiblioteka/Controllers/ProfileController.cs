using BFF.Auth.Keycloak;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dockerbiblioteka.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProfileController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        UserId = User.UserId(),
        Username = User.Username(),
        Email = User.Email(),
        EmailVerified = User.IsEmailVerified(),
        DisplayName = User.DisplayName(),
        GivenName = User.GivenName(),
        FamilyName = User.FamilyName(),
        Roles = User.KeycloakRoles()
    });

    [HttpGet("admin")]
    [Authorize(Roles = "admin")]
    public IActionResult GetAdmin() => Ok(new { Message = $"{User.Username()} ma rolę admin." });
}
