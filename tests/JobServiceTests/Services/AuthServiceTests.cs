using Xunit;
using Moq;
using DataAccess.Interfaces;
using DataAccess.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataAccess.Tests
{
	public class AuthServiceTests
	{
		private readonly IAuthService _authService;

		public AuthServiceTests()
		{
			// Mocked values for the tests
			var mockUsername = "admin";
			var mockPassword = "admin";
			var mockJwtKey = "this_is_a_very_secure_and_long_jwt_key_12345";

			_authService = new AuthService(mockUsername, mockPassword, mockJwtKey);
		}

		[Fact]
		public void Authenticate_ValidCredentials_ReturnsToken()
		{
			// Act
			var token = _authService.Authenticate("admin", "admin");

			// Assert
			Assert.NotNull(token);

			// Optional: Further validation of token structure
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("this_is_a_very_secure_and_long_jwt_key_12345");
			var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero
			}, out var validatedToken);

			Assert.NotNull(claimsPrincipal);
			Assert.IsType<JwtSecurityToken>(validatedToken);
		}

		[Fact]
		public void Authenticate_InvalidCredentials_ReturnsNull()
		{
			// Act
			var token = _authService.Authenticate("admin", "wrong_password");

			// Assert
			Assert.Null(token);
		}
	}
}
