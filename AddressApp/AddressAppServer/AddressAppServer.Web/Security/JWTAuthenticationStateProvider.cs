﻿using AddressAppServer.ClassLibrary.DTOs;
using Microsoft.AspNetCore.Components;
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
        private readonly NavigationManager _navigationManager;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public JWTAuthenticationStateProvider(ProtectedSessionStorage sessionStorage, NavigationManager navigationManager)
        {
            _sessionStorage = sessionStorage;
            _navigationManager = navigationManager;
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
                _navigationManager.NavigateTo("/login");
                return new AuthenticationState(_anonymous);
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(UserDTO userDto, string token, string refreshToken)
        {
            await _sessionStorage.SetAsync("accessToken", token);
            await _sessionStorage.SetAsync("refreshToken", refreshToken);
            await _sessionStorage.SetAsync("user", userDto);
            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _sessionStorage.DeleteAsync("accessToken");
            await _sessionStorage.DeleteAsync("refreshToken");
            await _sessionStorage.DeleteAsync("user");

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

        public async Task<string?> GetAccessTokenAsync()
        {
            var tokenResult = await _sessionStorage.GetAsync<string>("accessToken");
            return tokenResult.Success ? tokenResult.Value : null;
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            var tokenResult = await _sessionStorage.GetAsync<string>("refreshToken");
            return tokenResult.Success ? tokenResult.Value : null;
        }

        public async Task<UserDTO?> GetUserAsync()
        {
            var userResult = await _sessionStorage.GetAsync<UserDTO>("user");
            return userResult.Success ? userResult.Value : null;
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