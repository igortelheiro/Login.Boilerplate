using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MGR.Login.Application.Services.Interfaces
{
    public interface ITokenProviderService
    {
        string GenerateJwt(IdentityUser user);
        Task<string> RetrieveRefreshTokenAsync(IdentityUser user);
        Task<string> GenerateAndStoreRefreshTokenAsync(IdentityUser user);
    }
}
