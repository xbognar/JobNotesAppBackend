namespace DataAccess.Models
{
	public class Job
	{
		
		public int Id { get; set; } // Primary Key
		
		public int SerialNumber { get; set; } // P.C.
		
		public string JobNumber { get; set; } // Cislo Zakazky
		
		public string Location { get; set; } // Miesto
		
		public string? ClientName { get; set; } // Meno
		
		public DateTime? MeasurementDate { get; set; } // Datum merania
		
		public string? Notes { get; set; } // Poznamka
		
		public bool IsCompleted { get; set; } // Task completion status
	
	}
}
