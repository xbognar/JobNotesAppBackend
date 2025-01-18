using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DataAccess.Models;
using FluentAssertions;
using System.Net;

namespace IntegrationTests.Dependencies
{
	public static class TestUtilities
	{
		/// <summary>
		/// Logs in using environment-based credentials, returning a JWT token if successful.
		/// </summary>
		public static async Task<string> LoginAndGetTokenAsync(HttpClient client)
		{
			var envUser = System.Environment.GetEnvironmentVariable("AUTH_USERNAME") ?? "defaultUser";
			var envPass = System.Environment.GetEnvironmentVariable("AUTH_PASSWORD") ?? "defaultPass";

			var user = new User
			{
				Username = envUser,
				Password = envPass
			};

			var response = await client.PostAsJsonAsync("/api/auth/login", user);
			response.StatusCode.Should().Be(HttpStatusCode.OK, "Login must succeed if credentials match env variables");

			// The controller returns { Token = "..." }
			var body = await response.Content.ReadFromJsonAsync<dynamic>();
			string token = body.Token;
			token.Should().NotBeNullOrEmpty();

			return token;
		}

		/// <summary>
		/// Adds Bearer token to the HttpClient's request headers.
		/// </summary>
		public static void AddAuthToken(this HttpClient client, string token)
		{
			client.DefaultRequestHeaders.Remove("Authorization");
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		}
	}
}
