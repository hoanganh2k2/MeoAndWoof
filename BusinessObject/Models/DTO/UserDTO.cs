namespace BusinessObject.Models.DTO
{
	public class UserDTO
	{
		public int Userid { get; set; }

		public string Username { get; set; } = string.Empty;

		public string Password { get; set; } = string.Empty;

		public string? Fullname { get; set; } = string.Empty;

		public int? Gender { get; set; } = 0;

		public string? Email { get; set; } = string.Empty;

		public int Roleid { get; set; }

		public string? Address { get; set; } = string.Empty;
		public string? Userimage { get; set; } = string.Empty;
		public string? Sdt { get; set; } = string.Empty;

		public int? Status { get; set; } = 0;
	}
}
