using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Democrachat;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DemocrachatTest
{
    /// <summary>
    /// Authentication tests using a real database (not mocked).
    /// NOTE: THEREFORE REQUIRES ENVIRONMENT CONNECTION STRING
    /// </summary>
    public class DbAuthTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;

        public DbAuthTest(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task FullRegistrationProcess()
        {
            var usernameChars = "abcdefhijklmnopqrstuvwxyz";
            var rand = new Random();
            var username = "";
            for (int i = 0; i < 10; ++i) 
                username += usernameChars[rand.Next(usernameChars.Length)];
            
            await _client.PostAsync("/api/auth/register", null!);
            await _client.PostAsync("/api/auth/finalize", 
                JsonContent.Create(new { Username = username, Password = "thepassword"}));
            await _client.PostAsync("/api/auth/logout", null!);
            
            var loginResponse = await _client.PostAsync("/api/auth/login",
                JsonContent.Create(new { Username = username, Password = "notpassword" }));
            Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);
            loginResponse = await _client.PostAsync("/api/auth/login",
                JsonContent.Create(new { Username = username, Password = "thepassword" }));
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }
    }
}