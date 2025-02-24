using ApplicationCore.Migrations;
using Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System.IO.Packaging;
using System.Text;
using static QuestPDF.Helpers.Colors;

namespace ConsoleDev;

public class PropType
{
   public string Title { get; set; } = string.Empty;
   public string Code { get; set; } = string.Empty;
}
public class Property
{
   public string Code { get; set; } = string.Empty;
   public string Num { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;
   public bool Over { get; set; }
   public string Brand { get; set; } = string.Empty;
   public string Type { get; set; } = string.Empty;

   public string Unit { get; set; } = string.Empty;
   public string Location { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;

   public string MinUse { get; set; } = string.Empty;
   public string Used { get; set; } = string.Empty;

   public int GetNumber
   {
      get
      {
         if (string.IsNullOrEmpty(Num)) return 9999;
         
         var numericPart = new string(Num.Where(char.IsDigit).ToArray());
         if (string.IsNullOrEmpty(numericPart)) return 9999;

         return int.Parse(numericPart.Substring(numericPart.Length - 6));
      }
   }
   public int GetCodeNumber
   {
      get
      {
         if (string.IsNullOrEmpty(Code)) return 9999;
         var numericPart = new string(Code.Where(char.IsDigit).ToArray());
         return int.Parse(numericPart);
      }
   }
}

public class LCD
{
   public string Num { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
}
public class Trash
{
   public string Num { get; set; } = string.Empty;
   public string P_Num { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;
   public string Brand { get; set; } = string.Empty;
   public string PS { get; set; } = string.Empty;

   public int GetNumber
   {
      get 
      {
         var numericPart = new string(Num.Where(char.IsDigit).ToArray());
         return int.Parse(numericPart);
      }
   }
}
