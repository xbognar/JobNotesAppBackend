using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.DataAccess;
using System;

namespace IntegrationTests.Dependencies
{
	/// <summary>
	/// A custom fixture for integration tests.
	/// Configures environment = "IntegrationTest" so <c>Program.cs</c> can
	/// use InMemory EF or skip migrations, then seeds test data.
	/// </summary>
	public class IntegrationTestFixture : WebApplicationFactory<NoteAPI.Program>
	{
		/// <summary>
		/// Override the CreateHost to set environment variables
		/// and optionally seed the database.
		/// </summary>
		/// <param name="builder">The <see cref="IHostBuilder"/>.</param>
		/// <returns>The created <see cref="IHost"/>.</returns>
		protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.UseEnvironment("IntegrationTest");

			Environment.SetEnvironmentVariable("JWT_KEY", "IntegrationTestKey_Of_32_OrMore_Characters!!!");
			Environment.SetEnvironmentVariable("AUTH_USERNAME", "testUser");
			Environment.SetEnvironmentVariable("AUTH_PASSWORD", "testPass");

			var host = base.CreateHost(builder);

			using var scope = host.Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			db.Database.EnsureCreated();

			SeedDataHelper.Seed(db);

			return host;
		}
	}
}
