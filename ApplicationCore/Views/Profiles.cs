﻿using Infrastructure.Entities;
using Infrastructure.Views;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Views;

public class ProfilesViewModel
{
	public string UserId { get; set; } = String.Empty;

   public string Name { get; set; } = String.Empty;

   public string? Ps { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

}
