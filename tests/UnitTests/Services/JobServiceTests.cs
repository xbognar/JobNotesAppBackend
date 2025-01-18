using System;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using DataAccess.DataAccess;
using DataAccess.Models;
using DataAccess.Services;
using DataAccess.Services.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Services
{
	public class JobServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<Job>> _jobsDbSetMock;
		private readonly JobService _service;

		public JobServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_jobsDbSetMock = new Mock<DbSet<Job>>();
			_dbContextMock.Setup(db => db.Jobs).Returns(_jobsDbSetMock.Object);

			_service = new JobService(_dbContextMock.Object);
		}

		/// <summary>
		/// Tests retrieving all jobs.
		/// </summary>
		[Fact]
		public async Task GetJobsAsync_ReturnsAllJobs()
		{
			// Arrange
			var testJobs = new List<Job>
			{
				new Job { Id = 1 },
				new Job { Id = 2 }
			}.AsQueryable();

			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.Provider).Returns(testJobs.Provider);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.Expression).Returns(testJobs.Expression);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.ElementType).Returns(testJobs.ElementType);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.GetEnumerator()).Returns(testJobs.GetEnumerator());

			// Act
			var result = await _service.GetJobsAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests retrieving a job by ID if found.
		/// </summary>
		[Fact]
		public async Task GetJobByIdAsync_WhenFound_ReturnsJob()
		{
			// Arrange
			var job = new Job { Id = 10 };
			_dbContextMock.Setup(db => db.Jobs.FindAsync(10)).ReturnsAsync(job);

			// Act
			var result = await _service.GetJobByIdAsync(10);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(10);
		}

		/// <summary>
		/// Tests retrieving a job by ID when not found returns null.
		/// </summary>
		[Fact]
		public async Task GetJobByIdAsync_WhenNotFound_ReturnsNull()
		{
			_dbContextMock.Setup(db => db.Jobs.FindAsync(999)).ReturnsAsync((Job)null);

			var result = await _service.GetJobByIdAsync(999);

			result.Should().BeNull();
		}

		/// <summary>
		/// Tests adding a job and saving changes.
		/// </summary>
		[Fact]
		public async Task AddJobAsync_AddsAndSaves()
		{
			var newJob = new Job { Id = 5 };

			var result = await _service.AddJobAsync(newJob);

			_dbContextMock.Verify(db => db.Jobs.Add(newJob), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
			result.Should().Be(newJob);
		}

		/// <summary>
		/// Tests updating a job calls SaveChanges.
		/// </summary>
		[Fact]
		public async Task UpdateJobAsync_UpdatesAndSaves()
		{
			// Arrange
			var existing = new Job { Id = 20 };

			// Act
			await _service.UpdateJobAsync(existing);

			// Assert
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}


		/// <summary>
		/// Tests deleting a job that exists calls Remove and saves.
		/// </summary>
		[Fact]
		public async Task DeleteJobAsync_WhenFound_DeletesAndSaves()
		{
			var job = new Job { Id = 25 };
			_dbContextMock.Setup(db => db.Jobs.FindAsync(25)).ReturnsAsync(job);

			await _service.DeleteJobAsync(25);

			_dbContextMock.Verify(db => db.Jobs.Remove(job), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests deleting a job that doesn't exist does nothing.
		/// </summary>
		[Fact]
		public async Task DeleteJobAsync_WhenNotFound_DoesNothing()
		{
			_dbContextMock.Setup(db => db.Jobs.FindAsync(999)).ReturnsAsync((Job)null);

			await _service.DeleteJobAsync(999);

			_dbContextMock.Verify(db => db.Jobs.Remove(It.IsAny<Job>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}

		/// <summary>
		/// Tests counting jobs for a given year.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForYearAsync_ReturnsCorrectCount()
		{
			var jobData = new List<Job>
			{
				new Job { Id = 1, MeasurementDate = new DateTime(2023, 1, 1) },
				new Job { Id = 2, MeasurementDate = new DateTime(2023, 2, 2) },
				new Job { Id = 3, MeasurementDate = new DateTime(2022, 6, 6) }
			}.AsQueryable();

			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.Provider).Returns(jobData.Provider);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.Expression).Returns(jobData.Expression);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.ElementType).Returns(jobData.ElementType);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.GetEnumerator()).Returns(jobData.GetEnumerator());

			var result = await _service.GetJobsCountForYearAsync(2023);

			result.Should().Be(2);
		}

		/// <summary>
		/// Tests counting jobs for a specific month in a given year.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForMonthAsync_ReturnsCorrectCount()
		{
			var jobData = new List<Job>
			{
				new Job { Id = 1, MeasurementDate = new DateTime(2023, 2, 1) },
				new Job { Id = 2, MeasurementDate = new DateTime(2023, 2, 10) },
				new Job { Id = 3, MeasurementDate = new DateTime(2023, 3, 10) }
			}.AsQueryable();

			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.Provider).Returns(jobData.Provider);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.Expression).Returns(jobData.Expression);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.ElementType).Returns(jobData.ElementType);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.GetEnumerator()).Returns(jobData.GetEnumerator());

			var result = await _service.GetJobsCountForMonthAsync(2023, 2);
			result.Should().Be(2);
		}

		/// <summary>
		/// Tests searching jobs with optional filters (location, clientName, notes).
		/// </summary>
		[Fact]
		public async Task SearchJobsAsync_FiltersProperly()
		{
			var jobData = new List<Job>
			{
				new Job { Id = 1, Location = "CityA", ClientName = "Alice", Notes = "Test" },
				new Job { Id = 2, Location = "CityB", ClientName = "Bob", Notes = "Another" }
			}.AsQueryable();

			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.Provider).Returns(jobData.Provider);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.Expression).Returns(jobData.Expression);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.ElementType).Returns(jobData.ElementType);
			_jobsDbSetMock.As<IQueryable<Job>>().Setup(m => m.GetEnumerator()).Returns(jobData.GetEnumerator());

			// Filter by location
			var result = await _service.SearchJobsAsync("CityA", null, null);
			result.Should().HaveCount(1);

			// Filter by clientName
			result = await _service.SearchJobsAsync(null, "Bob", null);
			result.Should().HaveCount(1);

			// Filter by notes
			result = await _service.SearchJobsAsync(null, null, "Test");
			result.Should().HaveCount(1);

			// Combined
			result = await _service.SearchJobsAsync("CityA", "Alice", "Test");
			result.Should().HaveCount(1);

			// No matches
			result = await _service.SearchJobsAsync("Nowhere", null, null);
			result.Should().BeEmpty();
		}
	}
}
