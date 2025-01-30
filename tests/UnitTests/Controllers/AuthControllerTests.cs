using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using NoteAPI.Controllers;
using DataAccess.Interfaces;
using DataAccess.Models;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace UnitTests.Controllers
{
	/// <summary>
	/// Unit tests for <c>AuthController</c>, verifying valid/invalid logins.
	/// </summary>
	public class AuthControllerTests
	{
		private readonly Mock<IAuthService> _authServiceMock;
		private readonly AuthController _controller;

		/// <summary>
		/// Initializes the <see cref="AuthControllerTests"/>, setting up a mock <see cref="IAuthService"/>.
		/// </summary>
		public AuthControllerTests()
		{
			_authServiceMock = new Mock<IAuthService>();
			_controller = new AuthController(_authServiceMock.Object);
		}

		/// <summary>
		/// Tests that valid credentials yield 200 OK with a non-empty "token".
		/// </summary>
		[Fact]
		public void Login_ValidCredentials_ReturnsOkWithToken()
		{
			/// AAA: Arrange
			var user = new User { Username = "testUser", Password = "testPass" };
			_authServiceMock
				.Setup(s => s.Authenticate("testUser", "testPass"))
				.Returns("fake_jwt_token");

			/// AAA: Act
			var result = _controller.Login(user);

			/// AAA: Assert
			var okResult = result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

		}

		/// <summary>
		/// Tests that invalid credentials yield 401 Unauthorized and no token.
		/// </summary>
		[Fact]
		public void Login_InvalidCredentials_ReturnsUnauthorized()
		{
			/// AAA: Arrange
			var user = new User { Username = "wrong", Password = "wrong" };
			_authServiceMock
				.Setup(s => s.Authenticate("wrong", "wrong"))
				.Returns((string?)null);

			/// AAA: Act
			var result = _controller.Login(user);

			/// AAA: Assert
			var unauthorizedResult = result as UnauthorizedResult;
			unauthorizedResult.Should().NotBeNull();
			unauthorizedResult.StatusCode.Should().Be(401);
		}
	}
}
