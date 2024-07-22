using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Models;

public class Calendar : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public virtual ICollection<EventCalendar>? EventCalendars { get; set; }
}


public class EventCalendar
{
   public int EventId { get; set; }

   [Required]
   public virtual Event? Event { get; set; }

   public int CalendarId { get; set; }
   [Required]
   public virtual Calendar? Calendar { get; set; }
}

