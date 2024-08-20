using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace ApplicationCore.Models;

public class Attachment : EntityBase, IBaseUploadFile, IBaseRecord, IRemovable, ISortable
{
	public string PostType { get; set; } = String.Empty;
   public int PostId { get; set; }
   public string? Title { get; set; }
   public string? Description { get; set; }
   public string OriFileName { get; set; } = String.Empty;

   public string FileName { get; set; } = String.Empty;
   public string Ext { get; set; } = String.Empty;
   public long FileSize { get; set; }
   public string Host { get; set; } = String.Empty;
   public string DirectoryPath { get; set; } = String.Empty;


   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

}
