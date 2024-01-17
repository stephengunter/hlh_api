using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Consts;

namespace ApplicationCore.Models;

public class OAuth : EntityBase
{
	public string UserId { get; set; } = String.Empty;

	public string? Name { get; set; }

	public string? FamilyName { get; set; }

	public string? GivenName { get; set; }

	public string? OAuthId { get; set; }

	public OAuthProvider Provider { get; set; }

	public string? PictureUrl { get; set; }

	[Required]
	public virtual User? User { get; set; }
}
