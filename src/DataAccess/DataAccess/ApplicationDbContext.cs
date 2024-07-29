using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DataAccess.DataAccess
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Job> Jobs { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
