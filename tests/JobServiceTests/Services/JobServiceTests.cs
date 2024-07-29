using Xunit;
using DataAccess.Models;
using DataAccess.DataAccess;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Tests
{
	public class JobServiceTests
	{
		private readonly JobService _jobService;
		private readonly ApplicationDbContext _context;

		public JobServiceTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database for each test
				.Options;
			_context = new ApplicationDbContext(options);
			_jobService = new JobService(_context);
		}


		[Fact]
		public async Task GetJobsAsync_ReturnsAllJobs()
		{
			// Arrange
			_context.Jobs.AddRange(new List<Job>
			{
				new Job { SerialNumber = 1, JobNumber = "01-01/2024", Location = "Location1", ClientName = "Client1" },
				new Job { SerialNumber = 2, JobNumber = "02-01/2024", Location = "Location2", ClientName = "Client2" }
			});
			await _context.SaveChangesAsync();

			// Act
			var jobs = await _jobService.GetJobsAsync();

			// Assert
			Assert.Equal(2, jobs.Count());
		}


		[Fact]
		public async Task GetJobByIdAsync_ReturnsJob()
		{
			// Arrange
			var job = new Job { SerialNumber = 1, JobNumber = "01-01/2024", Location = "Location1", ClientName = "Client1" };
			_context.Jobs.Add(job);
			await _context.SaveChangesAsync();

			// Act
			var result = await _jobService.GetJobByIdAsync(job.Id);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(job.JobNumber, result.JobNumber);
		}

		[Fact]
		public async Task AddJobAsync_AddsJobToDatabase()
		{
			// Arrange
			var job = new Job { SerialNumber = 3, JobNumber = "03-01/2024", Location = "Location3", ClientName = "Client3" };

			// Act
			var addedJob = await _jobService.AddJobAsync(job);

			// Assert
			Assert.NotNull(addedJob);
			Assert.Equal("03-01/2024", addedJob.JobNumber);
			var dbJob = await _context.Jobs.FindAsync(addedJob.Id);
			Assert.NotNull(dbJob);
		}

		[Fact]
		public async Task UpdateJobAsync_UpdatesExistingJob()
		{
			// Arrange
			var job = new Job { SerialNumber = 4, JobNumber = "04-01/2024", Location = "Location4", ClientName = "Client4" };
			_context.Jobs.Add(job);
			await _context.SaveChangesAsync();

			// Act
			job.Location = "Updated Location";
			await _jobService.UpdateJobAsync(job);

			// Assert
			var updatedJob = await _context.Jobs.FindAsync(job.Id);
			Assert.Equal("Updated Location", updatedJob.Location);
		}

		[Fact]
		public async Task DeleteJobAsync_RemovesJobFromDatabase()
		{
			// Arrange
			var job = new Job { SerialNumber = 5, JobNumber = "05-01/2024", Location = "Location5", ClientName = "Client5" };
			_context.Jobs.Add(job);
			await _context.SaveChangesAsync();

			// Act
			await _jobService.DeleteJobAsync(job.Id);

			// Assert
			var deletedJob = await _context.Jobs.FindAsync(job.Id);
			Assert.Null(deletedJob);
		}

		[Fact]
		public async Task GetJobsCountForYearAsync_ReturnsCorrectCount()
		{
			// Arrange
			_context.Jobs.AddRange(new List<Job>
			{
				new Job { SerialNumber = 1, JobNumber = "01-01/2024", Location = "Location1", ClientName = "Client1", MeasurementDate = new DateTime(2024, 1, 1) },
				new Job { SerialNumber = 2, JobNumber = "02-01/2024", Location = "Location2", ClientName = "Client2", MeasurementDate = new DateTime(2024, 2, 1) },
				new Job { SerialNumber = 3, JobNumber = "03-01/2023", Location = "Location3", ClientName = "Client3", MeasurementDate = new DateTime(2023, 1, 1) }
			});
			await _context.SaveChangesAsync();

			// Act
			var count = await _jobService.GetJobsCountForYearAsync(2024);

			// Assert
			Assert.Equal(2, count);
		}

		[Fact]
		public async Task GetJobsCountForMonthAsync_ReturnsCorrectCount()
		{
			// Arrange
			_context.Jobs.AddRange(new List<Job>
			{
				new Job { SerialNumber = 1, JobNumber = "01-01/2024", Location = "Location1", ClientName = "Client1", MeasurementDate = new DateTime(2024, 1, 1) },
				new Job { SerialNumber = 2, JobNumber = "02-01/2024", Location = "Location2", ClientName = "Client2", MeasurementDate = new DateTime(2024, 1, 15) },
				new Job { SerialNumber = 3, JobNumber = "03-01/2024", Location = "Location3", ClientName = "Client3", MeasurementDate = new DateTime(2024, 2, 1) }
			});
			await _context.SaveChangesAsync();

			// Act
			var count = await _jobService.GetJobsCountForMonthAsync(2024, 1);

			// Assert
			Assert.Equal(2, count);
		}

		[Fact]
		public async Task SearchJobsAsync_ReturnsMatchingJobs()
		{
			// Arrange
			_context.Jobs.AddRange(new List<Job>
			{
				new Job { SerialNumber = 1, JobNumber = "01-01/2024", Location = "Location1", ClientName = "Client1", Notes = "Important job" },
				new Job { SerialNumber = 2, JobNumber = "02-01/2024", Location = "Location2", ClientName = "Client2", Notes = "Urgent job" },
				new Job { SerialNumber = 3, JobNumber = "03-01/2024", Location = "Location1", ClientName = "Client3", Notes = "Regular job" }
			});
			await _context.SaveChangesAsync();

			// Act
			var jobs = await _jobService.SearchJobsAsync("Location1", null, null);

			// Assert
			Assert.Equal(2, jobs.Count());
			Assert.All(jobs, job => Assert.Equal("Location1", job.Location));
		}
	}
}
