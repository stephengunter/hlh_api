using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;

public class Profile : BaseRecord
{
	public string UserId { get; set; } = String.Empty;

   public string Name { get; set; } = String.Empty;

   [Required]
	public virtual User? User { get; set; }
}
