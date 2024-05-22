using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;


public class AttachmentViewModel : EntityBaseView, IBaseUploadFileView, IBaseRecordView
{
   public string PostType { get; set; } = string.Empty;
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
   public bool Active { get; set; }
   
   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();

}
