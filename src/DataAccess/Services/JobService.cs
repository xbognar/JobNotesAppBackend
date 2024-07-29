using DataAccess.Models;
using DataAccess.DataAccess;
using Microsoft.EntityFrameworkCore;
using DataAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Services
{
	public class JobService : IJobService
	{
		private readonly ApplicationDbContext _context;

		public JobService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Job>> GetJobsAsync()
		{
			return await _context.Jobs.ToListAsync();
		}

		public async Task<Job> GetJobByIdAsync(int id)
		{
			return await _context.Jobs.FindAsync(id);
		}

		public async Task<Job> AddJobAsync(Job job)
		{
			_context.Jobs.Add(job);
			await _context.SaveChangesAsync();
			return job;
		}

		public async Task UpdateJobAsync(Job job)
		{
			_context.Entry(job).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteJobAsync(int id)
		{
			var job = await _context.Jobs.FindAsync(id);
			if (job != null)
			{
				_context.Jobs.Remove(job);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<int> GetJobsCountForYearAsync(int year)
		{
			return await _context.Jobs
				.CountAsync(j => j.MeasurementDate.HasValue && j.MeasurementDate.Value.Year == year);
		}

		public async Task<int> GetJobsCountForMonthAsync(int year, int month)
		{
			return await _context.Jobs
				.CountAsync(j => j.MeasurementDate.HasValue && j.MeasurementDate.Value.Year == year && j.MeasurementDate.Value.Month == month);
		}

		public async Task<IEnumerable<Job>> SearchJobsAsync(string? location, string? clientName, string? notes)
		{
			var query = _context.Jobs.AsQueryable();

			if (!string.IsNullOrEmpty(location))
			{
				query = query.Where(j => j.Location.Contains(location));
			}

			if (!string.IsNullOrEmpty(clientName))
			{
				query = query.Where(j => j.ClientName.Contains(clientName));
			}

			if (!string.IsNullOrEmpty(notes))
			{
				query = query.Where(j => j.Notes.Contains(notes));
			}

			return await query.ToListAsync();
		}
	}
}
