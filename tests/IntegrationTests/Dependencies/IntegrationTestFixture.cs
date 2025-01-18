using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DataAccess.DataAccess;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace IntegrationTests.Dependencies
{
	public class IntegrationTestFixture : WebApplicationFactory<Program>
	{
		protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				// Remove the existing DbContext registration (SQL)
				var descriptor = services.SingleOrDefault(
					d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
				if (descriptor != null)
					services.Remove(descriptor);

				// Register InMemory DB
				services.AddDbContext<ApplicationDbContext>(options =>
				{
					options.UseInMemoryDatabase("IntegrationTestDb");
				});

				// Build the service provider
				var sp = services.BuildServiceProvider();

				// Seed the test data
				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var db = scopedServices.GetRequiredService<ApplicationDbContext>();
					db.Database.EnsureCreated();
					SeedDataHelper.Seed(db);
				}
			});

			return base.CreateHost(builder);
		}
	}
}
