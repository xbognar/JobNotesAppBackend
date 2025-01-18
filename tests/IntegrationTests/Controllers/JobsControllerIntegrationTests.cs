using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegrationTests.Dependencies;
using DataAccess.Models;
using System.Collections.Generic;

namespace IntegrationTests.Controllers
{
	public class JobsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public JobsControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests retrieving all jobs requires authentication.
		/// </summary>
		[Fact]
		public async Task GetAllJobs_AsAuthenticatedUser_ReturnsOkAndList()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/jobs");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var jobs = await response.Content.ReadFromJsonAsync<List<Job>>();
			jobs.Should().NotBeNull();
			jobs.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Tests retrieving a job by ID after login. Expect seeded job #100 or #101.
		/// </summary>
		[Fact]
		public async Task GetJob_WhenFound_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/jobs/100");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var job = await response.Content.ReadFromJsonAsync<Job>();
			job.Should().NotBeNull();
			job.Id.Should().Be(100);
		}

		/// <summary>
		/// Tests requesting a non-existing job returns NotFound.
		/// </summary>
		[Fact]
		public async Task GetJob_WhenNotFound_ReturnsNotFound()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/jobs/9999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a new job returns Created, then fetch to confirm.
		/// </summary>
		[Fact]
		public async Task PostJob_ReturnsCreated()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var newJob = new Job
			{
				Id = 999,
				SerialNumber = 9999,
				JobNumber = "INTEGRATION-999",
				Location = "IntegrationCity",
				ClientName = "IntegrationClient",
				MeasurementDate = System.DateTime.UtcNow,
				Notes = "Created in test",
				IsCompleted = false
			};

			var response = await _client.PostAsJsonAsync("/api/jobs", newJob);
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			var fetch = await _client.GetAsync("/api/jobs/999");
			fetch.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		/// <summary>
		/// Tests updating an existing job returns NoContent.
		/// </summary>
		[Fact]
		public async Task PutJob_WhenIdMatches_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var updatedJob = new Job
			{
				Id = 101,
				SerialNumber = 8888,
				JobNumber = "UPDATED-101",
				Location = "UpdatedCity",
				ClientName = "UpdatedClient",
				IsCompleted = true
			};

			var response = await _client.PutAsJsonAsync("/api/jobs/101", updatedJob);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Tests mismatched ID returns BadRequest.
		/// </summary>
		[Fact]
		public async Task PutJob_WhenIdMismatch_ReturnsBadRequest()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var mismatchJob = new Job { Id = 500 };

			var response = await _client.PutAsJsonAsync("/api/jobs/600", mismatchJob);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Tests deleting a job returns NoContent (if found) or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteJob_WhenFoundOrNotFound_ReturnsCorrectStatus()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.DeleteAsync("/api/jobs/999");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests retrieving the count of jobs for a given year.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForYear_ReturnsOkWithCount()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/jobs/count/year/2023");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var count = await response.Content.ReadFromJsonAsync<int>();
			count.Should().BeGreaterThanOrEqualTo(0);
		}

		/// <summary>
		/// Tests retrieving the count for a given month.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForMonth_ReturnsOkWithCount()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/jobs/count/year/2023/month/2");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var count = await response.Content.ReadFromJsonAsync<int>();
			count.Should().BeGreaterThanOrEqualTo(0);
		}

		/// <summary>
		/// Tests searching for jobs with optional query parameters.
		/// </summary>
		[Fact]
		public async Task SearchJobs_ReturnsOkWithList()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/jobs/search?location=TestCity");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var jobs = await response.Content.ReadFromJsonAsync<List<Job>>();
			jobs.Should().NotBeNull();
			jobs.Count.Should().BeGreaterThan(0);
		}
	}
}
