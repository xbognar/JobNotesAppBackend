using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Dependencies;
using DataAccess.Models;

namespace IntegrationTests.Controllers
{
	public class AuthControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public AuthControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests login with valid environment credentials returns a token.
		/// </summary>
		[Fact]
		public async Task Login_ValidCredentials_ReturnsOkAndToken()
		{
			var envUser = System.Environment.GetEnvironmentVariable("AUTH_USERNAME") ?? "defaultUser";
			var envPass = System.Environment.GetEnvironmentVariable("AUTH_PASSWORD") ?? "defaultPass";

			var user = new User
			{
				Username = envUser,
				Password = envPass
			};

			var response = await _client.PostAsJsonAsync("/api/auth/login", user);
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var json = await response.Content.ReadFromJsonAsync<dynamic>();
			((string)json.Token).Should().NotBeNullOrEmpty();
		}

		/// <summary>
		/// Tests login with invalid credentials returns Unauthorized.
		/// </summary>
		[Fact]
		public async Task Login_InvalidCredentials_ReturnsUnauthorized()
		{
			var user = new User
			{
				Username = "wrongUser",
				Password = "wrongPass"
			};

			var response = await _client.PostAsJsonAsync("/api/auth/login", user);
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}
	}
}
