using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using NoteAPI.Controllers; // or correct namespace
using DataAccess.Services.Interfaces;
using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
	public class JobsControllerTests
	{
		private readonly Mock<IJobService> _jobServiceMock;
		private readonly JobsController _controller;

		public JobsControllerTests()
		{
			_jobServiceMock = new Mock<IJobService>();
			_controller = new JobsController(_jobServiceMock.Object);
		}

		/// <summary>
		/// Tests that GetJobs returns OkObjectResult with a list of jobs.
		/// </summary>
		[Fact]
		public async Task GetJobs_ReturnsOkWithList()
		{
			var jobs = new List<Job> { new Job(), new Job() };
			_jobServiceMock.Setup(s => s.GetJobsAsync()).ReturnsAsync(jobs);

			var result = await _controller.GetJobs();

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedList = Assert.IsType<List<Job>>(okResult.Value);
			returnedList.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that GetJob returns Ok when found.
		/// </summary>
		[Fact]
		public async Task GetJob_WhenFound_ReturnsOk()
		{
			var job = new Job { Id = 10 };
			_jobServiceMock.Setup(s => s.GetJobByIdAsync(10)).ReturnsAsync(job);

			var result = await _controller.GetJob(10);

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedJob = Assert.IsType<Job>(okResult.Value);
			returnedJob.Id.Should().Be(10);
		}

		/// <summary>
		/// Tests that GetJob returns NotFound if job does not exist.
		/// </summary>
		[Fact]
		public async Task GetJob_WhenNotFound_ReturnsNotFound()
		{
			_jobServiceMock.Setup(s => s.GetJobByIdAsync(999)).ReturnsAsync((Job)null);

			var result = await _controller.GetJob(999);

			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// Tests that PostJob returns CreatedAtAction with the new job.
		/// </summary>
		[Fact]
		public async Task PostJob_ReturnsCreatedAtAction()
		{
			var newJob = new Job { Id = 5 };
			_jobServiceMock.Setup(s => s.AddJobAsync(newJob)).ReturnsAsync(newJob);

			var result = await _controller.PostJob(newJob);

			var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			createdResult.RouteValues["id"].Should().Be(5);
			createdResult.Value.Should().Be(newJob);
		}

		/// <summary>
		/// Tests that PutJob returns NoContent if ID matches.
		/// </summary>
		[Fact]
		public async Task PutJob_WhenIdMatches_ReturnsNoContent()
		{
			var existingJob = new Job { Id = 10 };
			_jobServiceMock.Setup(s => s.UpdateJobAsync(existingJob)).Returns(Task.CompletedTask);

			var result = await _controller.PutJob(10, existingJob);

			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// Tests that PutJob returns BadRequest if ID mismatch.
		/// </summary>
		[Fact]
		public async Task PutJob_WhenIdMismatch_ReturnsBadRequest()
		{
			var job = new Job { Id = 5 };

			var result = await _controller.PutJob(999, job);

			Assert.IsType<BadRequestResult>(result);
		}

		/// <summary>
		/// Tests that DeleteJob returns NoContent when deletion is successful.
		/// </summary>
		[Fact]
		public async Task DeleteJob_ReturnsNoContent()
		{
			_jobServiceMock.Setup(s => s.DeleteJobAsync(10)).Returns(Task.CompletedTask);

			var result = await _controller.DeleteJob(10);

			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// Tests that GetJobsCountForYear returns Ok with an integer count.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForYear_ReturnsOkWithCount()
		{
			_jobServiceMock.Setup(s => s.GetJobsCountForYearAsync(2023)).ReturnsAsync(5);

			var result = await _controller.GetJobsCountForYear(2023);

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var count = Assert.IsType<int>(okResult.Value);
			count.Should().Be(5);
		}

		/// <summary>
		/// Tests that GetJobsCountForMonth returns Ok with an integer count.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForMonth_ReturnsOkWithCount()
		{
			_jobServiceMock.Setup(s => s.GetJobsCountForMonthAsync(2023, 5)).ReturnsAsync(3);

			var result = await _controller.GetJobsCountForMonth(2023, 5);

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var count = Assert.IsType<int>(okResult.Value);
			count.Should().Be(3);
		}

		/// <summary>
		/// Tests that SearchJobs returns Ok with a list.
		/// </summary>
		[Fact]
		public async Task SearchJobs_ReturnsOkWithList()
		{
			var foundJobs = new List<Job> { new Job { Id = 1 } };
			_jobServiceMock.Setup(s => s.SearchJobsAsync("loc", null, null)).ReturnsAsync(foundJobs);

			var result = await _controller.SearchJobs("loc", null, null);

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var list = Assert.IsType<List<Job>>(okResult.Value);
			list.Should().HaveCount(1);
		}
	}
}
