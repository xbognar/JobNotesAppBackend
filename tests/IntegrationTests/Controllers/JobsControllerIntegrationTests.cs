using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Dependencies;
using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTests.Controllers
{
	/// <summary>
	/// Integration tests for <c>JobsController</c>, ensuring end-to-end functionality with JWT authentication.
	/// </summary>
	public class JobsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		/// <summary>
		/// Initializes <see cref="JobsControllerIntegrationTests"/> with a fresh <see cref="HttpClient"/>.
		/// </summary>
		/// <param name="fixture">The fixture setting up "IntegrationTest" environment and seeded data.</param>
		public JobsControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests retrieving all jobs as an authenticated user returns 200 OK and a list of jobs.
		/// </summary>
		[Fact]
		public async Task GetAllJobs_ReturnsOkWithList()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			/// AAA: Act
			var response = await _client.GetAsync("/api/jobs");

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK, "authenticated requests should succeed");
			var jobs = await response.Content.ReadFromJsonAsync<List<Job>>();
			jobs.Should().NotBeNull("the controller should return JSON array of jobs");
			jobs.Count.Should().BeGreaterThan(0, "we seeded at least 2 jobs (#100, #101)");
		}

		/// <summary>
		/// Tests retrieving an existing seeded job (ID=100) returns 200 OK with its details.
		/// </summary>
		[Fact]
		public async Task GetJob_WhenFound_ReturnsOk()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			/// AAA: Act
			var response = await _client.GetAsync("/api/jobs/100");

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK, "job #100 is seeded data");
			var job = await response.Content.ReadFromJsonAsync<Job>();
			job.Should().NotBeNull();
			job.Id.Should().Be(100);
		}

		/// <summary>
		/// Tests that requesting a non-existent job returns 404 NotFound.
		/// </summary>
		[Fact]
		public async Task GetJob_WhenNotFound_ReturnsNotFound()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			/// AAA: Act
			var response = await _client.GetAsync("/api/jobs/9999");

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a new job via POST returns 201 Created, 
		/// then confirms it can be retrieved.
		/// </summary>
		[Fact]
		public async Task PostJob_ReturnsCreated()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var newJob = new Job
			{
				SerialNumber = 7777,
				JobNumber = "INTEG-777",
				Location = "TestLocation",
				ClientName = "TestClient",
				Notes = "Created in integration test"
			};

			/// AAA: Act
			var postResponse = await _client.PostAsJsonAsync("/api/jobs", newJob);

			/// AAA: Assert
			postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
			var created = await postResponse.Content.ReadFromJsonAsync<Job>();
			created.Should().NotBeNull();
			created.Id.Should().BeGreaterThan(0, "the DB should assign an ID");

			var getResponse = await _client.GetAsync($"/api/jobs/{created.Id}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		/// <summary>
		/// Tests updating a seeded job (#101) returns 204 NoContent on success.
		/// </summary>
		[Fact]
		public async Task PutJob_WhenIdMatches_ReturnsNoContent()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var updatedJob = new Job
			{
				Id = 101,
				SerialNumber = 2020,
				JobNumber = "UPDATED-101",
				Location = "UpdatedLoc",
				ClientName = "UpdatedClient",
				Notes = "Updated note",
				IsCompleted = false
			};

			/// AAA: Act
			var putResponse = await _client.PutAsJsonAsync("/api/jobs/101", updatedJob);

			/// AAA: Assert
			putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Tests that if the route ID doesn't match the job's ID, the response is 400 BadRequest.
		/// </summary>
		[Fact]
		public async Task PutJob_WhenIdMismatch_ReturnsBadRequest()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var mismatch = new Job { Id = 999 };

			/// AAA: Act
			var response = await _client.PutAsJsonAsync("/api/jobs/101", mismatch);

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Tests deleting a job returns 204 NoContent if it exists or 404 if not found 
		/// (the actual behavior depends on your controller logic).
		/// </summary>
		[Fact]
		public async Task DeleteJob_WhenFoundOrNotFound_ReturnsProperStatus()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			/// AAA: Act
			var response = await _client.DeleteAsync("/api/jobs/9999");

			/// AAA: Assert
			response.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.NotFound, HttpStatusCode.NoContent },
				"the controller may either return 404 if not found, or 204 if it quietly does nothing");
		}

		/// <summary>
		/// Tests retrieving the count of jobs for a given year returns 200 OK with an integer.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForYear_ReturnsOkWithCount()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			/// AAA: Act
			var response = await _client.GetAsync("/api/jobs/count/year/2023");

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var count = await response.Content.ReadFromJsonAsync<int>();
			count.Should().BeGreaterThanOrEqualTo(0, "the controller returns some integer count");
		}

		/// <summary>
		/// Tests retrieving the count of jobs for a particular month in a year also returns an integer.
		/// </summary>
		[Fact]
		public async Task GetJobsCountForMonth_ReturnsOkWithCount()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			/// AAA: Act
			var response = await _client.GetAsync("/api/jobs/count/year/2023/month/5");

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var count = await response.Content.ReadFromJsonAsync<int>();
			count.Should().BeGreaterThanOrEqualTo(0);
		}

		/// <summary>
		/// Tests searching for jobs by optional query parameters returns an OK status and a list.
		/// </summary>
		[Fact]
		public async Task SearchJobs_ReturnsOkWithList()
		{
			/// AAA: Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			/// AAA: Act
			var response = await _client.GetAsync("/api/jobs/search?location=SeedCity1");

			/// AAA: Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var foundJobs = await response.Content.ReadFromJsonAsync<List<Job>>();
			foundJobs.Should().NotBeNull();
		}
	}
}
