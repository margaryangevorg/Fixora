using Fixora.DAL.Entities;

namespace Fixora.API.Services.Interfaces;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(AppUser user);
}