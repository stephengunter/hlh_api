using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace ApplicationCore.Models;
public enum PostType
{
	Event = 0,
   Article = 1,
   Reference = 2,
   None = -1
}

public class Attachment : EntityBase, IBaseUploadFile, IBaseRecord, IRemovable, ISortable
{
	public PostType PostType { get; set; } = PostType.None;
	public int PostId { get; set; }
   public string? PreviewPath { get; set; }
   public int Width { get; set; }
   public int Height { get; set; }
   public string? Type { get; set; }
   public string? Title { get; set; }
   public string? Description { get; set; }

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
