using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using NoteAPI.Controllers; // or correct namespace
using DataAccess.Interfaces;
using DataAccess.Models;

namespace UnitTests.Controllers
{
	public class AuthControllerTests
	{
		private readonly Mock<IAuthService> _authServiceMock;
		private readonly AuthController _controller;

		public AuthControllerTests()
		{
			_authServiceMock = new Mock<IAuthService>();
			_controller = new AuthController(_authServiceMock.Object);
		}

		/// <summary>
		/// Tests that valid credentials return Ok with a token.
		/// </summary>
		[Fact]
		public void Login_ValidCredentials_ReturnsOkWithToken()
		{
			// Arrange
			var user = new User { Username = "testUser", Password = "testPass" };
			_authServiceMock.Setup(s => s.Authenticate("testUser", "testPass")).Returns("fake_jwt_token");

			// Act
			var result = _controller.Login(user);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			dynamic body = okResult.Value;
			((string)body.Token).Should().Be("fake_jwt_token");
		}

		/// <summary>
		/// Tests that invalid credentials return Unauthorized.
		/// </summary>
		[Fact]
		public void Login_InvalidCredentials_ReturnsUnauthorized()
		{
			// Arrange
			var user = new User { Username = "wrong", Password = "wrong" };
			_authServiceMock.Setup(s => s.Authenticate("wrong", "wrong")).Returns((string)null);

			// Act
			var result = _controller.Login(user);

			// Assert
			var unauthorized = Assert.IsType<UnauthorizedResult>(result);
			unauthorized.StatusCode.Should().Be(401);
		}
	}
}
