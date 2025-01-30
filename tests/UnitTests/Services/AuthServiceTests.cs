using Xunit;
using FluentAssertions;
using System;
using DataAccess.Services;

namespace UnitTests.Services
{
	/// <summary>
	/// Unit tests for <c>AuthService</c>, verifying JWT token creation and credential checks.
	/// </summary>
	public class AuthServiceTests
	{
		private const string USER = "testUser";
		private const string PASS = "testPass";
		private const string KEY = "MyUltraSecretKeyOf32CharactersOrLonger!!";

		private readonly AuthService _authService;

		/// <summary>
		/// Initializes <see cref="AuthServiceTests"/>, creating an <see cref="AuthService"/> with test credentials.
		/// </summary>
		public AuthServiceTests()
		{
			_authService = new AuthService(USER, PASS, KEY);
		}

		/// <summary>
		/// Tests that valid credentials produce a non-null JWT token.
		/// </summary>
		[Fact]
		public void Authenticate_ValidCredentials_ReturnsToken()
		{
			/// AAA: Act
			var token = _authService.Authenticate(USER, PASS);

			/// AAA: Assert
			token.Should().NotBeNullOrEmpty("valid credentials should generate a JWT token");
		}

		/// <summary>
		/// Tests that invalid credentials return null instead of a JWT.
		/// </summary>
		[Fact]
		public void Authenticate_InvalidCredentials_ReturnsNull()
		{
			/// AAA: Arrange, Act
			var token = _authService.Authenticate("wrong", "wrong");

			/// AAA: Assert
			token.Should().BeNull("wrong credentials should fail authentication");
		}

		/// <summary>
		/// Tests that the constructor throws <see cref="ArgumentNullException"/> if any parameter is null.
		/// </summary>
		[Fact]
		public void Constructor_WhenNullParams_ThrowsArgumentNullException()
		{
			/// AAA: Arrange
			Action nullUser = () => new AuthService(default!, PASS, KEY);
			Action nullPass = () => new AuthService(USER, default!, KEY);
			Action nullKey = () => new AuthService(USER, PASS, default!);

			/// AAA: Assert
			nullUser.Should().Throw<ArgumentNullException>().WithParameterName("username");
			nullPass.Should().Throw<ArgumentNullException>().WithParameterName("password");
			nullKey.Should().Throw<ArgumentNullException>().WithParameterName("jwtKey");
		}
	}
}
