using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Dependencies;
using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTests.Controllers
{
	/// <summary>
	/// Integration tests for <c>AuthController</c>, verifying authentication logic in a full pipeline.
	/// </summary>
	public class AuthControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		/// <summary>
		/// Creates a new <see cref="AuthControllerIntegrationTests"/> instance,
		/// initializing an <see cref="HttpClient"/> from <see cref="IntegrationTestFixture"/>.
		/// </summary>
		/// <param name="fixture">The fixture that sets up the integration environment.</param>
		public AuthControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests that valid credentials (from environment variables) return a 200 OK with a non-empty token.
		/// </summary>
		[Fact]
		public async Task Login_ValidCredentials_ReturnsOkAndToken()
		{
			/// AAA: Arrange
			var user = new User
			{
				Username = System.Environment.GetEnvironmentVariable("AUTH_USERNAME") ?? "testUser",
				Password = System.Environment.GetEnvironmentVariable("AUTH_PASSWORD") ?? "testPass"
			};

			/// AAA: Act
			var response = await _client.PostAsJsonAsync("/api/auth/login", user);

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK, "matching credentials should succeed");
			var dict = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
			dict.Should().NotBeNull();
			dict.Should().ContainKey("token", "the controller typically returns {\"token\":\"...\"}");

			var token = dict["token"];
			token.Should().NotBeNullOrEmpty("a successful login must return a JWT token");
		}

		/// <summary>
		/// Tests that invalid credentials return a 401 Unauthorized status.
		/// </summary>
		[Fact]
		public async Task Login_InvalidCredentials_ReturnsUnauthorized()
		{
			/// AAA: Arrange
			var user = new User
			{
				Username = "wrongUser",
				Password = "wrongPass"
			};

			/// AAA: Act
			var response = await _client.PostAsJsonAsync("/api/auth/login", user);

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, "invalid credentials should fail");
			var bodyText = await response.Content.ReadAsStringAsync();
			bodyText.Should().Contain("Unauthorized", "the response typically includes the status reason");
		}
	}
}
