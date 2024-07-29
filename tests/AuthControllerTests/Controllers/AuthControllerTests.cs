using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Interfaces;
using NoteAPI.Controllers;
using DataAccess.Models;

namespace NoteAPI.Tests
{
	public class AuthControllerTests
	{
		private readonly AuthController _authController;
		private readonly Mock<IAuthService> _mockAuthService;

		public AuthControllerTests()
		{
			_mockAuthService = new Mock<IAuthService>();
			_authController = new AuthController(_mockAuthService.Object);
		}

		[Fact]
		public void Login_ValidCredentials_ReturnsToken()
		{
			// Arrange
			var user = new User { Username = "admin", Password = "admin" };
			_mockAuthService.Setup(service => service.Authenticate(user.Username, user.Password))
				.Returns("fake-jwt-token");

			// Act
			var result = _authController.Login(user);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var response = okResult.Value;

			// Ensure response contains a token field
			Assert.NotNull(response);
			var tokenField = response.GetType().GetProperty("Token");
			Assert.NotNull(tokenField);

			var token = tokenField.GetValue(response);
			Assert.Equal("fake-jwt-token", token);
		}


		[Fact]
		public void Login_InvalidCredentials_ReturnsUnauthorized()
		{
			// Arrange
			var user = new User { Username = "admin", Password = "wrong-password" };
			_mockAuthService.Setup(service => service.Authenticate(user.Username, user.Password))
				.Returns((string)null);

			// Act
			var result = _authController.Login(user);

			// Assert
			Assert.IsType<UnauthorizedResult>(result);
		}
	}
}
