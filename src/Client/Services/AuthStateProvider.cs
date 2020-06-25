using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Agile4SMB.Shared.DTO;
using Microsoft.AspNetCore.Components.Authorization;

namespace Agile4SMB.Client.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private static string _authToken = null;
        public static string AuthToken => _authToken;

        private  HttpClient _http;

        public AuthStateProvider(HttpClient http)
        {
            _http = http;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (string.IsNullOrEmpty(AuthToken))
                return GenerateEmptyAuthenticationState();

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user"),}, "Bearer");
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
        
        private Task<AuthenticationState> GenerateEmptyAuthenticationState() => Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

        public async Task<bool> Authenticate(string username, string password)
        {
            var result =
                await _http.PostAsJsonAsync("api/token", new LoginDTO
                {
                    UserName = username, 
                    Password = password
                });

            if (!result.IsSuccessStatusCode)
                return false;

            var tokenResult = await result.Content.ReadFromJsonAsync<TokenDTO>();

            _authToken = tokenResult.AccessToken;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return true;
        }

        public async Task SignOut()
        {
            _authToken = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
