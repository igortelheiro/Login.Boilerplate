using System.Threading.Tasks;
using Login.Domain;
using Microsoft.AspNetCore.Identity;

namespace Login.Application.Services.Interfaces
{
    public interface ITokenProviderService
    {
        Task<string> GenerateJwt(ApplicationUser user);
        Task<string> GenerateAndStoreTokenAsync(ApplicationUser user, TokenPurpose tokenPurpose);
        Task<bool> ValidateTokenAsync(ApplicationUser user, TokenPurpose tokenPurpose, string token);
    }
}
