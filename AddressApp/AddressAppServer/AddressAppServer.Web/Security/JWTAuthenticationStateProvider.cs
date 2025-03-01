using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.Web.Services;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net.Http.Headers;
using System.Security.Claims;
namespace AddressAppServer.Web.Security
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ILogger<JWTAuthenticationStateProvider> _logger;
        private readonly IAuthClient _authClient;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private bool _isInitialized;

        public JWTAuthenticationStateProvider(ProtectedSessionStorage sessionStorage, ILogger<JWTAuthenticationStateProvider> logger, IAuthClient authClient)
        {
            _sessionStorage = sessionStorage;
            _logger = logger;
            _authClient = authClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!_isInitialized)
            {
                _logger.LogInformation("Authentication state not initialized.");
                return new AuthenticationState(_anonymous);
            }

            var tokenResult = await _sessionStorage.GetAsync<string>("accessToken");

            if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value))
            {
                _logger.LogInformation("No access token found.");
                return new AuthenticationState(_anonymous);
            }

            var token = tokenResult.Value;
            if (IsTokenExpired(token))
            {
                _logger.LogInformation("Token is expired. Attempting to refresh.");
                Result<RefreshUserTokenResponseDTO> result = await RefreshAccessTokenAsync();
                if (result.Success)
                {
                    await MarkUserAsAuthenticated(result.Value.User, result.Value.Token, result.Value.RefreshToken);
                }
                else
                {
                    _logger.LogInformation("Failed to refresh token. Marking user as logged out.");
                    await MarkUserAsLoggedOut();
                    return new AuthenticationState(_anonymous);
                }
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            _logger.LogInformation("User authenticated with claims: {Claims}", string.Join(", ", user.Claims.Select(c => $"{c.Type}: {c.Value}")));
            return new AuthenticationState(user);
        }

        public async Task InitializeAsync()
        {
            _isInitialized = true;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsAuthenticated(UserDTO userDto, string token, string refreshToken)
        {
            await _sessionStorage.SetAsync("accessToken", token);
            await _sessionStorage.SetAsync("refreshToken", refreshToken);
            await _sessionStorage.SetAsync("user", userDto);
            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));

            _logger.LogInformation("User marked as authenticated with claims: {Claims}", string.Join(", ", user.Claims.Select(c => $"{c.Type}: {c.Value}")));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task RefreshAuthenticated(string token, string refreshToken)
        {
            await _sessionStorage.SetAsync("accessToken", token);
            await _sessionStorage.SetAsync("refreshToken", refreshToken);
            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));

            _logger.LogInformation("User marked as authenticated with claims: {Claims}", string.Join(", ", user.Claims.Select(c => $"{c.Type}: {c.Value}")));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _sessionStorage.DeleteAsync("accessToken");
            await _sessionStorage.DeleteAsync("refreshToken");
            await _sessionStorage.DeleteAsync("user");

            _logger.LogInformation("User marked as logged out.");
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
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);
            return jwt.ValidTo < DateTime.UtcNow;
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var jwtHandler = new JsonWebTokenHandler();
            var jwtToken = jwtHandler.ReadJsonWebToken(token);

            return jwtToken.Claims;
        }

        private async Task<Result<RefreshUserTokenResponseDTO>> RefreshAccessTokenAsync()
        {
            var refreshTokenResult = await _sessionStorage.GetAsync<string>("refreshToken");
            if (!refreshTokenResult.Success || string.IsNullOrEmpty(refreshTokenResult.Value))
            {
                _logger.LogInformation("No refresh token found.");
                return null;
            }
            var userDto = await _sessionStorage.GetAsync<UserDTO>("user");

            var refreshToken = refreshTokenResult.Value;

            var result =  await _authClient.RefreshTokenAsync(userDto.Value, refreshToken);

            return result;

        }
    }
}