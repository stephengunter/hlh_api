using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace ApplicationCore.Models;
public enum PostType
{
	Article = 0,
	None = -1
}

public class Attachment : EntityBase, IBaseUploadFile, IBaseRecord, IRemovable, ISortable
{
	public PostType PostType { get; set; } = PostType.None;
	public int PostId { get; set; }
   public string? Path { get; set; }
   public string? PreviewPath { get; set; }
   public int Width { get; set; }
   public int Height { get; set; }
   public string? Type { get; set; }
   public string? Name { get; set; }
   public string? Title { get; set; }
   public string? Description { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

}
