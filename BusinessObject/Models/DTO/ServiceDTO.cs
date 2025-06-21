namespace BusinessObject.Models.DTO
{
	public class ServiceDTO
	{
		public int Serviceid { get; set; }

		public string Servicename { get; set; } = null!;

		public int Servicetypeid { get; set; }

		public string? Description { get; set; }

		public int Userid { get; set; }

		public string? Address { get; set; }

		public string? Serviceimage { get; set; }

		public double? Servicestar { get; set; }

		public int ServiceNumberReview { get; set; }

		public int ServiceNumberBooking { get; set; }

		public int? Status { get; set; }
		public List<Serviceimage>? Images { get; set; }
		public Serviceprice Serviceprice { get; set; }

		public Servicepettype Servicepettype { get; set; }
	}
}
