using MGR.Login.Infra.Users;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MGR.Login.Application.Services.Interfaces
{
    public interface ITokenProviderService
    {
        string GenerateJwt(ApplicationUser user);
        Task<string> RetrieveRefreshTokenAsync(ApplicationUser user);
        Task<string> GenerateAndStoreRefreshTokenAsync(ApplicationUser user);
    }
}
