using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
namespace AddressAppServer.Web.Security
{
    public class AddressAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IStorageService _storageService;
        private readonly NavigationManager _navigationManager;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public AddressAuthenticationStateProvider(IStorageService storageService, NavigationManager navigationManager)
        {
            _storageService = storageService;
            _navigationManager = navigationManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _storageService.GetAccessTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(_anonymous);
            }

            if (IsTokenExpired(token))
            {
                _navigationManager.NavigateTo("/login");
                return new AuthenticationState(_anonymous);
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(UserDTO userDto, string token, string refreshToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            await _storageService.SetAccessTokenAsync(token);
            await _storageService.SetRefreshTokenAsync(refreshToken);
            await _storageService.SetUserAsync(userDto);
            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _storageService.ClearStorageAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        public async Task<bool> SetAuthorizationHeaderAsync(HttpClient httpClient)
        {
            var token = await _storageService.GetAccessTokenAsync();
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return true;
            }
            return false;
        }

        private static bool IsTokenExpired(string token)
        {
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);
            return jwt.ValidTo < DateTime.UtcNow;
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var jwtHandler = new JsonWebTokenHandler();
            var jwtToken = jwtHandler.ReadJsonWebToken(token);

            return jwtToken.Claims;
        }
    }
}