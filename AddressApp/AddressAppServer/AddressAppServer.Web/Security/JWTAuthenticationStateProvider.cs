using AddressAppServer.ClassLibrary.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AddressAppServer.Web.Security
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public JWTAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenResult = await _sessionStorage.GetAsync<string>("accessToken");

            if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value))
            {
                return new AuthenticationState(_anonymous);
            }

            var token = tokenResult.Value;
            if (IsTokenExpired(token))
            {
                return new AuthenticationState(_anonymous);
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string token, string refreshToken)
        {
            await _sessionStorage.SetAsync("accessToken", token);
            await _sessionStorage.SetAsync("refreshToken", refreshToken);
            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _sessionStorage.DeleteAsync("accessToken");
            await _sessionStorage.DeleteAsync("refreshToken");

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        public async Task<bool> SetAuthorizationHeaderAsync(HttpClient httpClient)
        {
            var tokenResult = await _sessionStorage.GetAsync<string>("accessToken");
            if (tokenResult.Success && tokenResult.Value != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
                return true;
            }
            return false;
        }

        private static bool IsTokenExpired(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.ValidTo < DateTime.UtcNow;
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            return jwtToken.Claims;
        }
    }
}