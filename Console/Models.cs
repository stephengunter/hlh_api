using ApplicationCore.Migrations;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System.IO.Packaging;
using System.Text;
using static QuestPDF.Helpers.Colors;

namespace ConsoleDev;

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
