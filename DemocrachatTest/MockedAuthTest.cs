using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Democrachat;
using Democrachat.Auth;
using Democrachat.Auth.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DemocrachatTest
{
    /// <summary>
    /// Authentication tests using a mocked authorization service
    /// </summary>
    public class MockedAuthTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private Mock<IAuthService> _authServiceMock;

        public MockedAuthTest(WebApplicationFactory<Startup> factory)
        {
            _authServiceMock = new Mock<IAuthService>();                   
            _authServiceMock.Setup(s => s.AttemptLogin("username", "password"))
                .Returns(new UserData { Username = "username", Id = 10});
            _authServiceMock.Setup(s => s.RegisterUser())
                .Returns(new RegistrationResult(10));
            _authServiceMock.Setup(s => s.GetUserById(10))
                .Returns(new UserData { Username = "username", Id = 10});
            _authServiceMock.Setup(s => s.IsUsernameTaken("username")).Returns(true);
            // Simulate user finalization
            _authServiceMock.Setup(s => s.FinalizeNewUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((int id, string username, string password) =>
                {
                    _authServiceMock.Setup(s => s.AttemptLogin(username, password))
                        .Returns(new UserData {Username = username, Id = id});
                    _authServiceMock.Setup(s => s.GetUserById(id))
                        .Returns(new UserData { Username = username, Id = id});
                });
            
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services => {
                    services.AddScoped(_ => _authServiceMock.Object); 
                });
            }).CreateClient();
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
                new StringContent("{\"username\": \"wrong\", \"password\": \"wrong\"}",
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
            Assert.Equal("{\"username\":\"username\"}", await response.Content.ReadAsStringAsync());
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
                new StringContent("{\"username\": \"johndoe\", \"password\": \"newuser\"}", Encoding.Default, "application/json"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            await _client.PostAsync("/api/auth/logout", null!);
            response = await _client.PostAsync("/api/auth/login",
                new StringContent("{\"username\": \"johndoe\", \"password\": \"newuser\"}",
                    Encoding.Default, "application/json"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task CannotFinalizeExistingUsername()
        {
            await _client.PostAsync("/api/auth/register", null!);
            var response = await _client.PostAsync("/api/auth/finalize", 
                new StringContent("{\"username\": \"username\", \"password\": \"existinguser\"}", Encoding.Default, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("already taken", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task CannotRegisterWhenLoggedIn()
        {
            await _client.PostAsync("/api/auth/login",
                JsonContent.Create(new {Username = "username", Password = "password"}));
            var response = await _client.PostAsync("/api/auth/register", null!);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}