using Fixora.API.Models;
using Fixora.API.Models.AuthModels;
using Fixora.API.Services.Interfaces;
using Fixora.DAL.Constants;
using Fixora.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Fixora.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return ServiceResult<AuthResponse>.Error("Passwords do not match.");

        var allowedRoles = new[] { Roles.Admin, Roles.Manager, Roles.MaintenanceEngineer };
        if (!allowedRoles.Contains(request.Role))
            return ServiceResult<AuthResponse>.Error($"Invalid role. Allowed: {string.Join(", ", allowedRoles)}");

        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return ServiceResult<AuthResponse>.Error(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, request.Role);
        var token = await _jwtService.GenerateTokenAsync(user);

        return ServiceResult<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = request.Role,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return ServiceResult<AuthResponse>.Error("Invalid email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return ServiceResult<AuthResponse>.Error("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _jwtService.GenerateTokenAsync(user);

        return ServiceResult<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = roles.FirstOrDefault() ?? string.Empty,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
    }
}
