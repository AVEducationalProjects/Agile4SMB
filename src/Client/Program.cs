using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Agile4SMB.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Agile4SMB.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp =>
            {
                var client = new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)};
                if(!string.IsNullOrEmpty(AuthStateProvider.AuthToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthStateProvider.AuthToken);
                return client;

            });
            builder.Services.AddScoped<UserUnitService>();
            builder.Services.AddScoped<BacklogService>();
            builder.Services.AddScoped<GoalsService>();

            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddApiAuthorization();

            await builder.Build().RunAsync();
        }
    }
}
