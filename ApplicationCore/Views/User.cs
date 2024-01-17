using ApplicationCore.Consts;

namespace ApplicationCore.Views;
public class UserViewModel
{
	public string Id { get; set; } = String.Empty;

	public string? Name { get; set; }

	public string UserName { get; set; } = String.Empty;

	public string Email { get; set; } = String.Empty;

	public string? PhoneNumber { get; set; }

	public DateTime CreatedAt { get; set; }	

	public string CreatedAtText => CreatedAt.ToString(DateTimeFormats.Default);

	
	public string? Roles { get; set; }

	public bool HasPassword { get; set; }
}
