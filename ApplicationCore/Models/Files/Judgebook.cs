using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using Ardalis.Specification;

namespace ApplicationCore.Models.Files;

[Table("Files.Judgebooks")]
public class JudgebookFile : EntityBase, IBaseUploadFile, IBaseRecord, IRemovable
{
   public JudgebookFile(string year = "", string category = "", string num = "", string? ps = "", string? type = "")
   {
      Year = CheckYear(year) ? year : "";
      Category = CheckCategory(category) ? category : "";
      Num = CheckNum(num) ? num.ToInt().FormatNumberWithLeadingZeros(6) : "";
      Ps = ps;
      Type = type;
   }
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

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   [NotMapped]
   public string FullPath => Path.Combine(DirectoryPath, FileName);
   //{
   //   return System.IO.Path.Combine(entity.DirectoryPath, entity.FileName);
   //}
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

