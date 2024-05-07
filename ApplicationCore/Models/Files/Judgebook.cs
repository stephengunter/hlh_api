using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using Ardalis.Specification;
using ApplicationCore.Consts;

namespace ApplicationCore.Models.Files;

public interface IJudgebookFile
{
   int Id { get; set; }
   public int TypeId { get; set; }
   string CourtType { get; set; }
   string Year { get; set; }
   string Category { get; set; }
   string Num { get; set; }
   string FileNumber { get; set; }
   string? Ps { get; set; }
}

[Table("Files.Judgebooks")]
public class JudgebookFile : EntityBase, IJudgebookFile, IBaseUploadFile, IBaseRecord, IRemovable
{
   public JudgebookFile(int typeId, string fileNumber , string courtType = "", string year = "", string category = "", string num = "", string? ps = "")
   {
      TypeId = typeId;
      FileNumber = CheckFileNumber(fileNumber) ? fileNumber : ""; ;
      CourtType = CheckCourtType(courtType) ? courtType.ToUpper() : "";
      Year = CheckYear(year) ? year : "";
      Category = CheckCategory(category) ? category : "";
      Num = CheckNum(num) ? num.ToInt().FormatNumberWithLeadingZeros(6) : "";
      Ps = ps;
   }

   public int TypeId { get; set; }
   public virtual JudgebookType Type { get; set; }
   public string FileNumber { get; set; } = String.Empty;

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

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   [NotMapped]
   public string FullPath => Path.Combine(DirectoryPath, FileName);
   public static bool CheckFileNumber(string val)
   {
      if (String.IsNullOrEmpty(val)) return false;
      return true;
   }
   public static bool CheckCourtType(string val)
   {
      if (String.IsNullOrEmpty(val)) return false;
      if (val.ToUpper() == JudgeCourtTypes.H) return true;
      return val.ToUpper() == JudgeCourtTypes.V;
   }
   public static bool CheckYear(string val)
   {
      if (String.IsNullOrEmpty(val)) return false;

      if (val.ToInt() == 0) return false;

      return val.Length >= 2 && val.Length <= 3; // Check if the length is between 2 and 3 characters
   }
   public static bool CheckCategory(string val)
   {
      if (String.IsNullOrEmpty(val)) return false;

      return true;
   }
   public static bool CheckNum(string val)
   {
      if (String.IsNullOrEmpty(val)) return false;

      if (val.ToInt() == 0) return false;

      return val.Length <= 6;
   }

}

[Table("Files.JudgebookTypes")]
public class JudgebookType : EntityBase, IBaseCategory<JudgebookType>, IRemovable, ISortable
{
   public virtual ICollection<JudgebookFile>? JudgebookFiles { get; set; }
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;

   public JudgebookType? Parent { get; set; }

   public int? ParentId { get; set; }

   public bool IsRootItem => ParentId is null;

   public ICollection<JudgebookType>? SubItems { get; set; }
   [NotMapped]
   public ICollection<int>? SubIds { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }

   public bool Active => ISortableHelpers.IsActive(this);

   public void LoadSubItems(IEnumerable<IBaseCategory<JudgebookType>> types) => BaseCategoriesHelpers.LoadSubItems(this, types);
}


