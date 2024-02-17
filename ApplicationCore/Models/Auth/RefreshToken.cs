using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;

public class RefreshToken : IAggregateRoot
{
	[Key]
	public string UserId { get; set; } = String.Empty;
	public string Token { get; set; } = String.Empty; 
	public DateTime LastUpdated { get; set; } = DateTime.Now;
   public DateTime Expires { get; set; }

	public string? RemoteIpAddress { get; set; }

	[Required]
	public virtual User? User { get; set; }

	public bool Active => DateTime.UtcNow <= Expires;
}
