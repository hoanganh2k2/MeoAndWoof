namespace BusinessObject.Models.DTO
{
	public class PetDTO
	{
		public int Petid { get; set; }

		public int Userid { get; set; }

		public string? Petname { get; set; }

		public int Pettypeid { get; set; }

		public int? Gender { get; set; }

		public string? Petimage { get; set; }

		public int? Status { get; set; }
		public virtual Pettype? Pettype { get; set; } = null!;

	}
}
