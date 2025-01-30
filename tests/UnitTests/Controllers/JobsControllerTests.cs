using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using NoteAPI.Controllers; // Adjust if needed
using DataAccess.Services.Interfaces;
using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
	/// <summary>
	/// Unit tests for <c>JobsController</c>, verifying controller CRUD logic with a mocked <see cref="IJobService"/>.
	/// </summary>
	public class JobsControllerTests
	{
		private readonly Mock<IJobService> _jobServiceMock;
		private readonly JobsController _controller;

		/// <summary>
		/// Initializes <see cref="JobsControllerTests"/>, creating a mock <see cref="IJobService"/> and the controller.
		/// </summary>
		public JobsControllerTests()
		{
			_jobServiceMock = new Mock<IJobService>();
			_controller = new JobsController(_jobServiceMock.Object);
		}

		/// <summary>
		/// Tests that GET /api/jobs returns 200 OK with a list of jobs.
		/// </summary>
		[Fact]
		public async Task GetJobs_ReturnsOkWithList()
		{
			/// AAA: Arrange
			var jobs = new List<Job> { new Job(), new Job() };
			_jobServiceMock.Setup(s => s.GetJobsAsync()).ReturnsAsync(jobs);

			/// AAA: Act
			var result = await _controller.GetJobs();

			/// AAA: Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedList = okResult.Value as List<Job>;
			returnedList.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that GET /api/jobs/{id} returns 200 OK if the job is found.
		/// </summary>
		[Fact]
		public async Task GetJob_WhenFound_ReturnsOk()
		{
			/// AAA: Arrange
			var job = new Job { Id = 10, JobNumber = "TEST" };
			_jobServiceMock.Setup(s => s.GetJobByIdAsync(10)).ReturnsAsync(job);

			/// AAA: Act
			var result = await _controller.GetJob(10);

			/// AAA: Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedJob = okResult.Value as Job;
			returnedJob.Id.Should().Be(10);
			returnedJob.JobNumber.Should().Be("TEST");
		}

		/// <summary>
		/// Tests that GET /api/jobs/{id} returns 404 if the job is not found.
		/// </summary>
		[Fact]
		public async Task GetJob_WhenNotFound_ReturnsNotFound()
		{
			/// AAA: Arrange
			_jobServiceMock.Setup(s => s.GetJobByIdAsync(999)).ReturnsAsync((Job)null);

			/// AAA: Act
			var result = await _controller.GetJob(999);

			/// AAA: Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that POST /api/jobs returns 201 Created with the newly added job.
		/// </summary>
		[Fact]
		public async Task PostJob_ReturnsCreatedAtAction()
		{
			/// AAA: Arrange
			var newJob = new Job { Id = 5, JobNumber = "NEW" };
			_jobServiceMock.Setup(s => s.AddJobAsync(newJob)).ReturnsAsync(newJob);

			/// AAA: Act
			var result = await _controller.PostJob(newJob);

			/// AAA: Assert
			var createdResult = result.Result as CreatedAtActionResult;
			createdResult.Should().NotBeNull();
			createdResult.StatusCode.Should().Be(201);
			createdResult.RouteValues["id"].Should().Be(5);
			createdResult.Value.Should().Be(newJob);
		}

		/// <summary>
		/// Tests that PUT /api/jobs/{id} returns 204 if IDs match and update succeeds.
		/// </summary>
		[Fact]
		public async Task PutJob_WhenIdMatches_ReturnsNoContent()
		{
			/// AAA: Arrange
			var job = new Job { Id = 10, JobNumber = "UPDATED" };
			_jobServiceMock.Setup(s => s.UpdateJobAsync(job)).Returns(Task.CompletedTask);

			/// AAA: Act
			var result = await _controller.PutJob(10, job);

			/// AAA: Assert
			result.Should().BeOfType<NoContentResult>();
		}

		/// <summary>
		/// Tests that PUT /api/jobs/{id} returns 400 if the route ID does not match the job's ID.
		/// </summary>
		[Fact]
		public async Task PutJob_WhenIdMismatch_ReturnsBadRequest()
		{
			/// AAA: Arrange
			var job = new Job { Id = 5 };

			/// AAA: Act
			var result = await _controller.PutJob(999, job);

			/// AAA: Assert
			result.Should().BeOfType<BadRequestResult>();
		}

		/// <summary>
		/// Tests that DELETE /api/jobs/{id} returns 204 NoContent, 
		/// or if not found, the service does nothing (so 204 or 404, depending on your logic).
		/// </summary>
		[Fact]
		public async Task DeleteJob_ReturnsNoContent()
		{
			/// AAA: Arrange
			_jobServiceMock.Setup(s => s.DeleteJobAsync(10)).Returns(Task.CompletedTask);

			/// AAA: Act
			var result = await _controller.DeleteJob(10);

			/// AAA: Assert
			result.Should().BeOfType<NoContentResult>();
		}

		/// <summary>
		/// Tests that GET /api/jobs/count/year/{year} returns 200 with the integer count.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForYear_ReturnsOkWithCount()
		{
			/// AAA: Arrange
			_jobServiceMock.Setup(s => s.GetJobsCountForYearAsync(2023)).ReturnsAsync(5);

			/// AAA: Act
			var result = await _controller.GetJobsCountForYear(2023);

			/// AAA: Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			var count = (int)okResult.Value;
			count.Should().Be(5);
		}

		/// <summary>
		/// Tests that GET /api/jobs/count/year/{year}/month/{month} returns 200 with the integer count.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForMonth_ReturnsOkWithCount()
		{
			/// AAA: Arrange
			_jobServiceMock.Setup(s => s.GetJobsCountForMonthAsync(2023, 5)).ReturnsAsync(3);

			/// AAA: Act
			var result = await _controller.GetJobsCountForMonth(2023, 5);

			/// AAA: Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			var count = (int)okResult.Value;
			count.Should().Be(3);
		}

		/// <summary>
		/// Tests that GET /api/jobs/search returns 200 OK with a filtered list of jobs.
		/// </summary>
		[Fact]
		public async Task SearchJobs_ReturnsOkWithList()
		{
			/// AAA: Arrange
			var foundJobs = new List<Job> { new Job { Id = 1 } };
			_jobServiceMock
				.Setup(s => s.SearchJobsAsync("loc", "client", "notes"))
				.ReturnsAsync(foundJobs);

			/// AAA: Act
			var result = await _controller.SearchJobs("loc", "client", "notes");

			/// AAA: Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			var list = okResult.Value as List<Job>;
			list.Should().HaveCount(1);
		}
	}
}
