using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Services.Interfaces;
using NoteAPI.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteAPI.Tests
{
	public class JobsControllerTests
	{
		private readonly JobsController _jobsController;
		private readonly Mock<IJobService> _mockJobService;

		public JobsControllerTests()
		{
			_mockJobService = new Mock<IJobService>();
			_jobsController = new JobsController(_mockJobService.Object);
		}

		[Fact]
		public async Task GetJobs_ReturnsOkResultWithJobs()
		{
			// Arrange
			var jobs = new List<Job>
			{
				new Job { Id = 1, JobNumber = "01-01/2024" },
				new Job { Id = 2, JobNumber = "02-01/2024" }
			};
			_mockJobService.Setup(service => service.GetJobsAsync()).ReturnsAsync(jobs);

			// Act
			var result = await _jobsController.GetJobs();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnJobs = Assert.IsType<List<Job>>(okResult.Value);
			Assert.Equal(2, returnJobs.Count);
		}

		[Fact]
		public async Task GetJob_ReturnsOkResultWithJob()
		{
			// Arrange
			var job = new Job { Id = 1, JobNumber = "01-01/2024" };
			_mockJobService.Setup(service => service.GetJobByIdAsync(1)).ReturnsAsync(job);

			// Act
			var result = await _jobsController.GetJob(1);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnJob = Assert.IsType<Job>(okResult.Value);
			Assert.Equal("01-01/2024", returnJob.JobNumber);
		}

		[Fact]
		public async Task GetJob_ReturnsNotFoundResult()
		{
			// Arrange
			_mockJobService.Setup(service => service.GetJobByIdAsync(1)).ReturnsAsync((Job)null);

			// Act
			var result = await _jobsController.GetJob(1);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task PostJob_ValidJob_ReturnsCreatedAtAction()
		{
			// Arrange
			var job = new Job { Id = 1, JobNumber = "01-01/2024" };
			_mockJobService.Setup(service => service.AddJobAsync(job)).ReturnsAsync(job);

			// Act
			var result = await _jobsController.PostJob(job);

			// Assert
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnJob = Assert.IsType<Job>(createdAtActionResult.Value);
			Assert.Equal("01-01/2024", returnJob.JobNumber);
		}

		[Fact]
		public async Task PutJob_ValidJob_ReturnsNoContent()
		{
			// Arrange
			var job = new Job { Id = 1, JobNumber = "01-01/2024" };
			_mockJobService.Setup(service => service.UpdateJobAsync(job)).Returns(Task.CompletedTask);

			// Act
			var result = await _jobsController.PutJob(1, job);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteJob_ExistingJob_ReturnsNoContent()
		{
			// Arrange
			_mockJobService.Setup(service => service.DeleteJobAsync(1)).Returns(Task.CompletedTask);

			// Act
			var result = await _jobsController.DeleteJob(1);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}
