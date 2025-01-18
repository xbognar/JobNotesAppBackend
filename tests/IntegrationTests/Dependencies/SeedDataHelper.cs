using DataAccess.DataAccess;
using DataAccess.Models;
using System;

namespace IntegrationTests.Dependencies
{
	public static class SeedDataHelper
	{
		public static void Seed(ApplicationDbContext db)
		{
			// If already seeded, skip
			if (db.Jobs.Any()) return;

			db.Jobs.AddRange(
				new Job
				{
					Id = 100,
					SerialNumber = 123,
					JobNumber = "JOB-100",
					Location = "TestCity",
					ClientName = "TestClient",
					MeasurementDate = DateTime.UtcNow,
					Notes = "Seed job #100",
					IsCompleted = false
				},
				new Job
				{
					Id = 101,
					SerialNumber = 456,
					JobNumber = "JOB-101",
					Location = "AnotherCity",
					ClientName = "AnotherClient",
					MeasurementDate = DateTime.UtcNow.AddDays(-5),
					Notes = "Seed job #101",
					IsCompleted = true
				}
			);

			db.SaveChanges();
		}
	}
}
