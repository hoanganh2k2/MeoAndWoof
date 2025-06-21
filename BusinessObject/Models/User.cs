using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models;

public partial class User
{
	public int Userid { get; set; }

	[Required(ErrorMessage = "Username is required")]
	[MinLength(6, ErrorMessage = "Username needs a minimum of 6 characters")]
	[MaxLength(20, ErrorMessage = "Username needs a maximum of 20 characters")]
	public string Username { get; set; } = null!;

	[Required(ErrorMessage = "Password is required")]
	[MinLength(6, ErrorMessage = "Password needs a minimum of 6 characters")]
	[MaxLength(20, ErrorMessage = "Password needs a maximum of 20 characters")]
	[RegularExpression(@"^(?=.*[A-Z]).+$", ErrorMessage = "Password must contain at least one uppercase letter")]
	public string Password { get; set; } = null!;

	[Required(ErrorMessage = "Full Name is required")]
	[RegularExpression(@"^[^\d]+$", ErrorMessage = "Full name cannot contain numbers")]
	public string? Fullname { get; set; }

	public int? Gender { get; set; }

	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Invalid email address")]
	public string? Email { get; set; }

	public string? Userimage { get; set; }

	public int Roleid { get; set; }

	[Required(ErrorMessage = "Address is required")]
	public string? Address { get; set; }

	[Required(ErrorMessage = "Phone number is required")]
	[RegularExpression(@"^(?:\+84|0)[3|5|7|8|9][0-9]{8}$", ErrorMessage = "Invalid phone number format")]
	public string? Sdt { get; set; }

	public int? Status { get; set; }

	/// <summary>
	/// Loại đăng nhập
	/// </summary>
	public string? Loginprovider { get; set; }

	public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

	public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

	public virtual ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();

	public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

	public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

	public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

	public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

	public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

	public virtual Role Role { get; set; } = null!;

	public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
