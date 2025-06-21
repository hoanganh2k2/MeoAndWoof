namespace BusinessObject.Models.DTO
{
	public class ServicePriceDTO
	{
		public int Servicepriceid { get; set; }

		public int Serviceid { get; set; }

		public DateTime Startdate { get; set; }

		public DateTime? Enddate { get; set; }

		public int? Pettypeid { get; set; }

		public double? Numberprice { get; set; }
		public virtual Pettype? Pettype { get; set; }
	}
}
