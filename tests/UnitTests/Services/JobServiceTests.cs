using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DataAccess.DataAccess;
using DataAccess.Models;
using DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Services
{
	/// <summary>
	/// Unit tests for <c>JobService</c>, using a real InMemory EF context to handle async queries properly.
	/// </summary>
	public class JobServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly JobService _jobService;
		private readonly string _databaseName;

		/// <summary>
		/// Sets up an in-memory EF context for each test, optionally seeding data.
		/// </summary>
		public JobServiceTests()
		{
			_databaseName = Guid.NewGuid().ToString();

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(_databaseName)
				.Options;

			_dbContext = new ApplicationDbContext(options);

			_dbContext.Jobs.AddRange(new List<Job>
			{
				new Job
				{
					Id = 1,
					SerialNumber = 100,
					JobNumber = "JOB-001",
					Location = "CityA",
					ClientName = "Alice",
					MeasurementDate = DateTime.UtcNow.AddDays(-2),
					Notes = "Seeded job #1",
					IsCompleted = false
				},
				new Job
				{
					Id = 2,
					SerialNumber = 200,
					JobNumber = "JOB-002",
					Location = "CityB",
					ClientName = "Bob",
					MeasurementDate = DateTime.UtcNow.AddDays(-10),
					Notes = "Seeded job #2",
					IsCompleted = true
				}
			});
			_dbContext.SaveChanges();

			_jobService = new JobService(_dbContext);
		}

		/// <summary>
		/// Cleans up the in-memory database after each test.
		/// </summary>
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		/// <summary>
		/// Tests that <see cref="JobService.GetJobsAsync"/> returns all seeded jobs.
		/// </summary>
		[Fact]
		public async Task GetJobsAsync_ReturnsAllJobs()
		{
			/// AAA: Act
			var result = await _jobService.GetJobsAsync();

			/// AAA: Assert
			result.Should().HaveCount(2, "we seeded two jobs (#1, #2)");
		}

		/// <summary>
		/// Tests that <see cref="JobService.GetJobByIdAsync"/> returns the correct job if found.
		/// </summary>
		[Fact]
		public async Task GetJobByIdAsync_WhenFound_ReturnsJob()
		{
			/// AAA: Act
			var job = await _jobService.GetJobByIdAsync(1);

			/// AAA: Assert
			job.Should().NotBeNull();
			job.Id.Should().Be(1);
		}

		/// <summary>
		/// Tests that <see cref="JobService.GetJobByIdAsync"/> returns null if the job doesn't exist.
		/// </summary>
		[Fact]
		public async Task GetJobByIdAsync_WhenNotFound_ReturnsNull()
		{
			/// AAA: Arrange, Act
			var job = await _jobService.GetJobByIdAsync(999);

			/// AAA: Assert
			job.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="JobService.AddJobAsync"/> inserts and saves a new job.
		/// </summary>
		[Fact]
		public async Task AddJobAsync_AddsAndSaves()
		{
			/// AAA: Arrange
			var newJob = new Job
			{
				Id = 10,
				SerialNumber = 9999,
				JobNumber = "JOB-010",
				Location = "NewCity"
			};

			/// AAA: Act
			var created = await _jobService.AddJobAsync(newJob);

			/// AAA: Assert
			created.Should().NotBeNull();
			created.Id.Should().Be(10);

			var inDb = await _dbContext.Jobs.FindAsync(10);
			inDb.Should().NotBeNull();
			inDb.JobNumber.Should().Be("JOB-010");
		}

		/// <summary>
		/// Tests that <see cref="JobService.UpdateJobAsync"/> updates a job and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateJobAsync_UpdatesAndSaves()
		{
			/// AAA: Arrange
			var existing = await _dbContext.Jobs.FindAsync(2);
			existing.Location = "UpdatedCity";

			/// AAA: Act
			await _jobService.UpdateJobAsync(existing);

			/// AAA: Assert
			var inDb = await _dbContext.Jobs.FindAsync(2);
			inDb.Location.Should().Be("UpdatedCity");
		}

		/// <summary>
		/// Tests that <see cref="JobService.DeleteJobAsync"/> removes a job if it exists.
		/// </summary>
		[Fact]
		public async Task DeleteJobAsync_WhenFound_DeletesAndSaves()
		{
			/// AAA: Arrange
			// job #1 or #2 is seeded

			/// AAA: Act
			await _jobService.DeleteJobAsync(1);

			/// AAA: Assert
			var inDb = await _dbContext.Jobs.FindAsync(1);
			inDb.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="JobService.DeleteJobAsync"/> does nothing if a job is not found.
		/// </summary>
		[Fact]
		public async Task DeleteJobAsync_WhenNotFound_DoesNothing()
		{
			/// AAA: Arrange
			// #999 doesn't exist

			/// AAA: Act
			await _jobService.DeleteJobAsync(999);

			/// AAA: Assert
			// Should not throw or remove anything
			_dbContext.Jobs.Should().HaveCount(2, "still only the 2 seeded jobs remain");
		}

		/// <summary>
		/// Tests that <see cref="JobService.GetJobsCountForYearAsync"/> counts only jobs in the specified year.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForYearAsync_ReturnsCorrectCount()
		{
			/// AAA: Arrange
			var year = DateTime.UtcNow.Year;

			/// AAA: Act
			var count = await _jobService.GetJobsCountForYearAsync(year);

			/// AAA: Assert
			count.Should().Be(2, "both seeded jobs are from the current year if you're using small negative days");
		}

		/// <summary>
		/// Tests that <see cref="JobService.GetJobsCountForMonthAsync"/> counts only jobs in the specified month/year.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForMonthAsync_ReturnsCorrectCount()
		{
			/// AAA: Arrange
			var year = DateTime.UtcNow.Year;
			var month = DateTime.UtcNow.Month;

			/// AAA: Act
			var count = await _jobService.GetJobsCountForMonthAsync(year, month);

			/// AAA: Assert
			count.Should().Be(2, "both seeded jobs fall in the same month if not crossing boundaries");
		}

		/// <summary>
		/// Tests that <see cref="JobService.SearchJobsAsync"/> filters jobs by location, clientName, notes, etc.
		/// </summary>
		[Fact]
		public async Task SearchJobsAsync_FiltersProperly()
		{
			/// AAA: Act & Assert
			var result = await _jobService.SearchJobsAsync("CityA", null, null);
			result.Should().HaveCount(1, "CityA matches job #1 only");

			result = await _jobService.SearchJobsAsync(null, "Bob", null);
			result.Should().HaveCount(1, "Bob is the client for job #2");

			result = await _jobService.SearchJobsAsync(null, null, "Seeded job #1");
			result.Should().HaveCount(1, "notes for job #1 matches 'Seeded job #1'");

			result = await _jobService.SearchJobsAsync("CityB", "Bob", "Seeded");
			result.Should().HaveCount(1, "all match job #2 only");

			result = await _jobService.SearchJobsAsync("NoCity", null, null);
			result.Should().BeEmpty("no match for 'NoCity'");
		}
	}
}
