using System.Threading.Tasks;
using Login.Domain;
using Microsoft.AspNetCore.Identity;

namespace Login.Application.Services.Interfaces
{
    public interface ITokenProviderService
    {
        Task<string> GenerateJwt(IdentityUser user);
        Task<string> GenerateAndStoreTokenAsync(IdentityUser user, TokenPurpose tokenPurpose);
        Task<bool> ValidateTokenAsync(IdentityUser user, TokenPurpose tokenPurpose, string token);
    }
}
