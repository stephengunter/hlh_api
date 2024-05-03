using ApplicationCore.Models;
using ApplicationCore.Models.Files;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.Files;
public class JudgebookFileViewModel : EntityBaseView, IJudgebookFile, IBaseRecordView
{
   public int TypeId { get; set; }
   public string CourtType { get; set; } = String.Empty;
   public string Year { get; set; } = String.Empty;
   public string Category { get; set; } = String.Empty;
   public string Num { get; set; } = String.Empty;
   public string? Ps { get; set; }

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

   public JudgebookTypeViewModel? Type { get; set; }
   public BaseFileView? FileView { get; set; }
}


public class JudgebookTypeViewModel : EntityBaseView, IBaseCategoryView<JudgebookTypeViewModel>
{
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public JudgebookTypeViewModel? Parent { get; set; }
   public int? ParentId { get; set; }
   public bool IsRootItem { get; set; }
   public ICollection<JudgebookTypeViewModel>? SubItems { get; set; }
   public ICollection<int>? SubIds { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }
}
