using DataAccess.DataAccess;
using DataAccess.Interfaces;
using DataAccess.Services;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NoteAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// 1) If environment = "IntegrationTest", use InMemory DB so we skip real SQL & migrations.
			//    Otherwise, read the connection string from environment vars and do SQL + migrations.
			if (builder.Environment.IsEnvironment("IntegrationTest"))
			{
				// Use an in-memory database with a unique name (avoid collisions across test runs)
				var uniqueDbName = $"IntegrationTestDb_{Guid.NewGuid()}";
				builder.Services.AddDbContext<ApplicationDbContext>(opts =>
					opts.UseInMemoryDatabase(uniqueDbName));
			}
			else
			{
				var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
					?? throw new InvalidOperationException("Missing CONNECTION_STRING environment variable.");

				builder.Services.AddDbContext<ApplicationDbContext>(opts =>
					opts.UseSqlServer(connectionString));
			}

			// 2) Read environment variables needed for Auth
			var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "fallbackKey_ChangeMe";
			var authUsername = Environment.GetEnvironmentVariable("AUTH_USERNAME") ?? "defaultUser";
			var authPassword = Environment.GetEnvironmentVariable("AUTH_PASSWORD") ?? "defaultPass";

			// 3) Register your app services
			builder.Services.AddScoped<IJobService, JobService>();
			builder.Services.AddScoped<IAuthService, AuthService>(_ =>
				new AuthService(authUsername, authPassword, jwtKey));

			// 4) Configure JWT
			var keyBytes = Encoding.ASCII.GetBytes(jwtKey);
			builder.Services.AddAuthentication(opts =>
			{
				opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(opts =>
			{
				opts.RequireHttpsMetadata = false; // OK for local dev/tests
				opts.SaveToken = true;
				opts.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

			// 5) Register controllers + swagger
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// 6) If NOT IntegrationTest => run migrations
			if (!app.Environment.IsEnvironment("IntegrationTest"))
			{
				using var scope = app.Services.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				db.Database.Migrate();
			}

			// 7) Setup middleware
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			app.Run();
		}
	}
}
