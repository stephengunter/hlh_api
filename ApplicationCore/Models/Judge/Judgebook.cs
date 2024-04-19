using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class Judgebook : EntityBase, IBaseRecord, IRemovable
{
   public string Year { get; set; } = String.Empty;
   public string Category { get; set; } = String.Empty;
   public string Num { get; set; } = String.Empty;
   public string? Ps { get; set; }
   public string? Type { get; set; }

   public string OriFileName { get; set; } = String.Empty;
   public string FileName { get; set; } = String.Empty;
   public string FileSize { get; set; } = String.Empty;

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; }
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

}

