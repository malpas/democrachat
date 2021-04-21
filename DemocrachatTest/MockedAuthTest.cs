using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Democrachat;
using Democrachat.Auth;
using Democrachat.Db.Models;
using DemocrachatTest.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DemocrachatTest
{
    /// <summary>
    /// Authentication tests using a mocked authorization service
    /// </summary>
    public class MockedAuthTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private IAuthService _authService;
        private WebApplicationFactory<Startup> _factory;
        private FakeUserService _fakeUserService;

        public MockedAuthTest(WebApplicationFactory<Startup> factory)
        {
            _fakeUserService = new FakeUserService();
            var hash = BCrypt.Net.BCrypt.HashPassword("password");
            _fakeUserService.UserData.Add(new UserData { Username = "username", Id = 10, Hash = hash});
            _authService = new AuthService(_fakeUserService);
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services => {
                    services.AddSingleton(_ => _authService); 
                });
            }).CreateClient();
            _factory = factory;
        }

        [Fact]
        public async Task GoodLoginWorks()
        {
            var response = await _client.PostAsync("/api/auth/login",
                new StringContent("{\"username\": \"username\", \"password\": \"password\"}",
                    Encoding.Default, "application/json"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BadLoginRejected()
        {
            var response = await _client.PostAsync("/api/auth/login",
                new StringContent("{\"username\": \"username\", \"password\": \"wrong\"}",
                    Encoding.Default, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RegistrationLogsInUser()
        {
            var response = await _client.PostAsync("/api/auth/register", null!);
            response = await _client.GetAsync("/api/auth/info");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CannotImmediatelyRegisterAgain()
        {
            var response = await _client.PostAsync("/api/auth/register", null!);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            await _client.PostAsync("/api/auth/logout", null!);
            response = await _client.PostAsync("/api/auth/register", null!);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("recently", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task CanGetUserInfo()
        {
            await _client.PostAsync("/api/auth/login",
                new StringContent("{\"username\": \"username\", \"password\": \"password\"}",
                    Encoding.Default, "application/json"));
            var response = await _client.GetAsync("/api/auth/info");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(new UserData { Username = "username", Gold = 0, Silver = 0}, 
                await response.Content.ReadFromJsonAsync<UserData>());
        }
        
        [Fact]
        public async Task MustBeLoggedInForUserInfo()
        {
            var response = await _client.GetAsync("/api/auth/info");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CanLogout()
        {
            await _client.PostAsync("/api/auth/login",
                new StringContent("{\"username\": \"username\", \"password\": \"password\"}",
                    Encoding.Default, "application/json"));
            await _client.PostAsync("/api/auth/logout", null!);
            var response = await _client.GetAsync("/api/auth/info");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CanConvertNewAccountToFullAccount()
        {
            await _client.PostAsync("/api/auth/register", null!);
            var response = await _client.PostAsync("/api/auth/finalize", 
                new StringContent("{\"username\": \"johndoe\", \"password\": \"newuserpass\"}", Encoding.Default, "application/json"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            await _client.PostAsync("/api/auth/logout", null!);
            response = await _client.PostAsync("/api/auth/login",
                new StringContent("{\"username\": \"johndoe\", \"password\": \"newuserpass\"}",
                    Encoding.Default, "application/json"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task CannotFinalizeExistingUsername()
        {
            await _client.PostAsync("/api/auth/register", null!);
            var response = await _client.PostAsync("/api/auth/finalize", 
                JsonContent.Create(new {Username = "username", Password = "existinguser"}));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("already taken", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task CannotFinalizeNonGuestAccount()
        {
            await _client.PostAsync("/api/auth/login",
                JsonContent.Create(new {Username = "username", Password = "password"}));
            var response = await _client.PostAsync("/api/auth/finalize",
                JsonContent.Create(new {Username = "newuser", Password = "newpassword"}));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CannotRegisterWhenLoggedIn()
        {
            await _client.PostAsync("/api/auth/login",
                JsonContent.Create(new {Username = "username", Password = "password"}));
            var response = await _client.PostAsync("/api/auth/register", null!);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }       
        
        [Fact]
        public async Task NaughtyWordsGiveFakeUsernameTakenMsg()
        {
            foreach (var naughtyWord in AuthService.NaughtyWordList)
            {
                _client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services => {
                        services.AddSingleton(_ => _authService); 
                    });
                }).CreateClient();
                await _client.PostAsync("/api/auth/register", null!);
                var response = await _client.PostAsync("/api/auth/finalize",
                    JsonContent.Create(new {Username = $"{naughtyWord}uu", Password = "passwordwersdf"}));
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Contains("taken", 
                    (await response.Content.ReadAsStringAsync()).ToLower());
            }
        }

        [Fact]
        public async Task LoginIsLogged()
        {
            await _client.PostAsync("/api/auth/login",
                JsonContent.Create(new {Username = "username", Password = "password"}));
            Assert.True(_fakeUserService.IsLoginLogged);
        }
    }
}