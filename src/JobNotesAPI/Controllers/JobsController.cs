using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Models;
using DataAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class JobsController : ControllerBase
	{
		private readonly IJobService _jobService;

		public JobsController(IJobService jobService)
		{
			_jobService = jobService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
		{
			var jobs = await _jobService.GetJobsAsync();
			return Ok(jobs);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Job>> GetJob(int id)
		{
			var job = await _jobService.GetJobByIdAsync(id);
			if (job == null)
			{
				return NotFound();
			}
			return Ok(job);
		}

		[HttpPost]
		public async Task<ActionResult<Job>> PostJob(Job job)
		{
			var createdJob = await _jobService.AddJobAsync(job);
			return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, createdJob);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutJob(int id, Job job)
		{
			if (id != job.Id)
			{
				return BadRequest();
			}

			await _jobService.UpdateJobAsync(job);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteJob(int id)
		{
			await _jobService.DeleteJobAsync(id);
			return NoContent();
		}

		[HttpGet("count/year/{year}")]
		public async Task<ActionResult<int>> GetJobsCountForYear(int year)
		{
			var count = await _jobService.GetJobsCountForYearAsync(year);
			return Ok(count);
		}

		[HttpGet("count/year/{year}/month/{month}")]
		public async Task<ActionResult<int>> GetJobsCountForMonth(int year, int month)
		{
			var count = await _jobService.GetJobsCountForMonthAsync(year, month);
			return Ok(count);
		}

		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<Job>>> SearchJobs(
			[FromQuery] string? location,
			[FromQuery] string? clientName,
			[FromQuery] string? notes)
		{
			var jobs = await _jobService.SearchJobsAsync(location, clientName, notes);
			return Ok(jobs);
		}
	}
}
