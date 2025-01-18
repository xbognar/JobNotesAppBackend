using System;
using Xunit;
using FluentAssertions;
using DataAccess.Services;
using DataAccess.Interfaces;

namespace UnitTests.Services
{
	public class AuthServiceTests
	{
		private const string USER = "testUser";
		private const string PASS = "testPass";
		private const string KEY = "testJwtKey1234";

		private readonly AuthService _authService;

		public AuthServiceTests()
		{
			_authService = new AuthService(USER, PASS, KEY);
		}

		/// <summary>
		/// Tests that valid credentials return a non-null JWT token.
		/// </summary>
		[Fact]
		public void Authenticate_ValidCredentials_ReturnsToken()
		{
			var token = _authService.Authenticate(USER, PASS);
			token.Should().NotBeNullOrEmpty();
		}

		/// <summary>
		/// Tests that invalid credentials return null.
		/// </summary>
		[Fact]
		public void Authenticate_InvalidCredentials_ReturnsNull()
		{
			var token = _authService.Authenticate("wrongUser", "wrongPass");
			token.Should().BeNull();
		}

		/// <summary>
		/// Tests constructor throws if username/password/jwtKey are null.
		/// </summary>
		[Fact]
		public void Constructor_WhenNullParams_ThrowsArgumentNull()
		{
			Action nullUser = () => new AuthService(null, PASS, KEY);
			Action nullPass = () => new AuthService(USER, null, KEY);
			Action nullKey = () => new AuthService(USER, PASS, null);

			nullUser.Should().Throw<ArgumentNullException>();
			nullPass.Should().Throw<ArgumentNullException>();
			nullKey.Should().Throw<ArgumentNullException>();
		}
	}
}
