using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net;
using DataAccess.Models;
using System.Collections.Generic;

namespace IntegrationTests.Dependencies
{
	/// <summary>
	/// Shared utility methods for integration tests,
	/// including logging in to retrieve a JWT token.
	/// </summary>
	public static class TestUtilities
	{
		/// <summary>
		/// Logs in using environment-based credentials (AUTH_USERNAME / AUTH_PASSWORD),
		/// returning the JWT token if the server responds with 200 OK.
		/// </summary>
		/// <param name="client">The <see cref="HttpClient"/> to send requests.</param>
		/// <returns>A JWT token string.</returns>
		public static async Task<string> LoginAndGetTokenAsync(HttpClient client)
		{
			/// AAA: Arrange
			var envUser = System.Environment.GetEnvironmentVariable("AUTH_USERNAME") ?? "testUser";
			var envPass = System.Environment.GetEnvironmentVariable("AUTH_PASSWORD") ?? "testPass";

			var user = new User
			{
				Username = envUser,
				Password = envPass
			};

			/// AAA: Act
			var response = await client.PostAsJsonAsync("/api/auth/login", user);

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK, "valid environment credentials should succeed");
			var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
			body.Should().NotBeNull("the response must be valid JSON");
			body.Should().ContainKey("token", "the JSON property is typically camel-cased by default");

			return body["token"];
		}

		/// <summary>
		/// Adds an Authorization header with the given bearer token to the <see cref="HttpClient"/>.
		/// </summary>
		/// <param name="client">The <see cref="HttpClient"/> for subsequent requests.</param>
		/// <param name="token">The JWT token string to use as Bearer authentication.</param>
		public static void AddAuthToken(this HttpClient client, string token)
		{
			client.DefaultRequestHeaders.Remove("Authorization");
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		}
	}
}
