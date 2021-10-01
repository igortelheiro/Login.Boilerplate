using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Models;
using MGR.Login.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MGR.Login.Application.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        #region Initialize
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginCommandHandler(IConfiguration configuration,
                                   UserManager<IdentityUser> userManager,
                                   SignInManager<IdentityUser> signInManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }
        #endregion


        public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await GetUserAsync(command.Email);

            await ValidateCredentialsAsync(user, command);

            var token = GetTokenForUser(user);
            return new LoginResult { Token = token };
        }


        private async Task<IdentityUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private async Task ValidateCredentialsAsync(IdentityUser user, LoginCommand command)
        {
            var credentialsValidation = await _signInManager
                .PasswordSignInAsync(user, command.Password, command.RememberMe,
                    lockoutOnFailure: false).ConfigureAwait(false);

            if (credentialsValidation.Succeeded == false)
            {
                var isEmailConfirmed = credentialsValidation.IsNotAllowed;
                var errorMessage = isEmailConfirmed ? "Senha inválida" : "Email não confirmado";
                throw new ArgumentException(errorMessage);
            }
        }


        private string GetTokenForUser(IdentityUser user)
        {
            var jwt = BuildJwt(user);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }


        private JwtSecurityToken BuildJwt(IdentityUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[Jwt.SecurityKey]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddHours(int.Parse(_configuration[Jwt.ExpiryInHours]));

            return new JwtSecurityToken(
                _configuration[Jwt.Issuer],
                _configuration[Jwt.Audience],
                claims,
                expires: expiry,
                signingCredentials: credentials
            );
        }
    }
}
