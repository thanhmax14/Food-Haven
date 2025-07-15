using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Food_Haven.Web;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Models.DBContext;
using Models;

namespace TestUnit
{
    [TestFixture]
    public class LoginTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                       
                        services.AddDbContext<FoodHavenDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestFoodHavenDb");
                        });

                    });
                });

            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        // Test case 1: Valid credentials
        [Test]
        public async Task Login_WithValidCredentials_ShouldReturnSuccessJson()
        {
            // Arrange
            await SeedTestUser("testuser@gmail.com", "Password123!", emailConfirmed: true);

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "testuser@gmail.com"),
                new KeyValuePair<string, string>("password", "Password123!"),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("success"));
            Assert.IsTrue(responseBody.Contains("Login successful"));
        }

        // Test case 2: Empty username
        [Test]
        public async Task Login_WithEmptyUsername_ShouldReturnError()
        {
            // Arrange
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", ""),
                new KeyValuePair<string, string>("password", "Password123!"),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("error"));
            Assert.IsTrue(responseBody.Contains("Username cannot be empty"));
        }

        // Test case 3: Empty password
        [Test]
        public async Task Login_WithEmptyPassword_ShouldReturnError()
        {
            // Arrange
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "testuser@gmail.com"),
                new KeyValuePair<string, string>("password", ""),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("error"));
            Assert.IsTrue(responseBody.Contains("Password cannot be empty"));
        }

        // Test case 4: Non-existent user
        [Test]
        public async Task Login_WithNonExistentUser_ShouldReturnError()
        {
            // Arrange
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "nonexistent@gmail.com"),
                new KeyValuePair<string, string>("password", "Password123!"),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("error"));
            Assert.IsTrue(responseBody.Contains("Account does not exist"));
        }

        // Test case 5: Banned user
        [Test]
        public async Task Login_WithBannedUser_ShouldReturnError()
        {
            // Arrange
            await SeedTestUser("banned@gmail.com", "Password123!", emailConfirmed: true, isBanned: true);

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "banned@gmail.com"),
                new KeyValuePair<string, string>("password", "Password123!"),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("error"));
            Assert.IsTrue(responseBody.Contains("locked by the administrator"));
        }

        // Test case 6: Unconfirmed email
        [Test]
        public async Task Login_WithUnconfirmedEmail_ShouldReturnError()
        {
            // Arrange
            await SeedTestUser("unconfirmed@gmail.com", "Password123!", emailConfirmed: false);

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "unconfirmed@gmail.com"),
                new KeyValuePair<string, string>("password", "Password123!"),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("error"));
            Assert.IsTrue(responseBody.Contains("verify your email"));
        }

        // Test case 7: Wrong password
        [Test]
        public async Task Login_WithWrongPassword_ShouldReturnError()
        {
            // Arrange
            await SeedTestUser("testuser@gmail.com", "Password123!", emailConfirmed: true);

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "testuser@gmail.com"),
                new KeyValuePair<string, string>("password", "WrongPassword!"),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("error"));
            Assert.IsTrue(responseBody.Contains("Incorrect password"));
        }

        // Test case 8: Test with ReturnUrl
        [Test]
        public async Task Login_WithReturnUrl_ShouldReturnCorrectRedirectUrl()
        {
            // Arrange
            await SeedTestUser("testuser@gmail.com", "Password123!", emailConfirmed: true);

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "testuser@gmail.com"),
                new KeyValuePair<string, string>("password", "Password123!"),
                new KeyValuePair<string, string>("rememberMe", "false"),
                new KeyValuePair<string, string>("ReturnUrl", "/Dashboard")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("success"));
            Assert.IsTrue(responseBody.Contains("/Dashboard"));
        }

        // Test case 9: Test login with username instead of email
        [Test]
        public async Task Login_WithUsername_ShouldReturnSuccess()
        {
            // Arrange
            await SeedTestUser("testuser@gmail.com", "Password123!", emailConfirmed: true, userName: "testuser");

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "testuser"),
                new KeyValuePair<string, string>("password", "Password123!"),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("success"));
        }

        // Test case 10: Test multiple failed attempts
        [Test]
        public async Task Login_WithMultipleFailedAttempts_ShouldShowRemainingAttempts()
        {
            // Arrange
           // await SeedTestUser("testuser@gmail.com", "Password123!", emailConfirmed: true);


            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "testuser@gmail.com"),
                new KeyValuePair<string, string>("password", "WrongPassword!"),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            // Act
            var response = await _client.PostAsync("/Home/Login", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(responseBody.Contains("error"));
            Assert.IsTrue(responseBody.Contains("attempts left"));
        }

        // Test case 11: Test GET Login returns view
        [Test]
        public async Task Login_GET_ShouldReturnLoginView()
        {
            // Act
            var response = await _client.GetAsync("/Home/Login");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.Content.Headers.ContentType.MediaType.Contains("text/html"));
        }

        // Test case 12: Test GET Login with ReturnUrl sets ViewData
        [Test]
        public async Task Login_GET_WithReturnUrl_ShouldSetViewData()
        {
            // Act
            var response = await _client.GetAsync("/Home/Login?ReturnUrl=/Dashboard");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            // ViewData sẽ được set trong controller để view sử dụng
        }

        // Helper method để tạo test user
        private async Task SeedTestUser(string email, string password, bool emailConfirmed = true, bool isBanned = false, string userName = null)
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var context = scope.ServiceProvider.GetRequiredService<FoodHavenDbContext>();

            // Đảm bảo database được tạo
            await context.Database.EnsureCreatedAsync();

            var user = new AppUser
            {
                UserName = userName ?? email,
                Email = email,
                EmailConfirmed = emailConfirmed,
                IsBannedByAdmin = isBanned,
             
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // Helper method để login user (nếu cần cho setup test khác)
        private async Task LoginUser(string email, string password)
        {
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("rememberMe", "false")
            });

            await _client.PostAsync("/Home/Login", formData);
        }
    }
}