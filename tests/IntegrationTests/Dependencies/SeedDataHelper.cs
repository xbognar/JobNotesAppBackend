using DataAccess.DataAccess;
using DataAccess.Models;
using System;

namespace IntegrationTests.Dependencies
{
	/// <summary>
	/// A static helper class to seed the test database with initial data.
	/// </summary>
	public static class SeedDataHelper
	{
		/// <summary>
		/// Populates the <see cref="ApplicationDbContext"/> with seed data if not already seeded.
		/// </summary>
		/// <param name="db">The <see cref="ApplicationDbContext"/> to populate.</param>
		public static void Seed(ApplicationDbContext db)
		{
			if (db.Jobs.Any()) return;

			db.Jobs.AddRange(
				new Job
				{
					Id = 100,
					SerialNumber = 1000,
					JobNumber = "JOB-100",
					Location = "SeedCity1",
					ClientName = "SeedClient1",
					MeasurementDate = DateTime.UtcNow.AddDays(-5),
					Notes = "IntegrationTest seeded job #100",
					IsCompleted = false
				},
				new Job
				{
					Id = 101,
					SerialNumber = 2000,
					JobNumber = "JOB-101",
					Location = "SeedCity2",
					ClientName = "SeedClient2",
					MeasurementDate = DateTime.UtcNow.AddDays(-10),
					Notes = "IntegrationTest seeded job #101",
					IsCompleted = true
				}
			);

			db.SaveChanges();
		}
	}
}
