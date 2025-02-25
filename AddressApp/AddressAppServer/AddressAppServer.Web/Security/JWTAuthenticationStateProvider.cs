using AddressAppServer.ClassLibrary.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AddressAppServer.Web.Security
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _protectedSessionStorage;

        public JWTAuthenticationStateProvider(ProtectedSessionStorage protectedSessionStorage)
        {
            _protectedSessionStorage = protectedSessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ProtectedBrowserStorageResult<string> token = await _protectedSessionStorage.GetAsync<string>("authToken");
            ProtectedBrowserStorageResult<string> refreshToken = await _protectedSessionStorage.GetAsync<string>("refreshToken");

            if (string.IsNullOrEmpty(token.Value))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token.Value);

            if (jwtToken == null || jwtToken.ValidTo < DateTime.UtcNow)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = jwtToken.Claims;
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
    }
}
