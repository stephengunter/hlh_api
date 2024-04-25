using ApplicationCore.Models;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.Files;
public class JudgebookFileViewModel : EntityBaseView, IBaseRecordView
{
   public string Year { get; set; } = String.Empty;
   public string Category { get; set; } = String.Empty;
   public string Num { get; set; } = String.Empty;
   public string? Ps { get; set; }
   public string? Type { get; set; }

   public string FileName { get; set; } = String.Empty;
   public string Ext { get; set; } = String.Empty;
   public long FileSize { get; set; }
   public string Host { get; set; } = String.Empty;
   public string DirectoryPath { get; set; } = String.Empty;

   public string FullPath { get; set; } = String.Empty;

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; }
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateTimeString();
   public string LastUpdatedText => LastUpdated.ToDateTimeString();
}

