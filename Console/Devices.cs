using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDev;
public class DeviceHelpers
{
   
   public static List<Property> GetProperties(string filePath, PropType type)
   {
      var properties = new List<Property>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[type.Code];
         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }
         int rowCount = worksheet.Dimension.Rows; // Total rows
         for (int row = 2; row <= rowCount; row++)
         {
            var prop = new Property()
            {
               Code = worksheet.Cells[row, 2].Text,
               Num = worksheet.Cells[row, 3].Text.Trim(),
               Name = worksheet.Cells[row, 4].Text,
               Brand = worksheet.Cells[row, 5].Text
            };
            properties.Add(prop);
         }
      }
      return properties;
   }
   public static void OutputProps(List<Property> properties, string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("sheet1");

         // Add headers
         worksheet.Cells[1, 1].Value = "";
         worksheet.Cells[1, 2].Value = "設備編號";
         worksheet.Cells[1, 3].Value = "財產編號";
         worksheet.Cells[1, 4].Value = "名稱";
         worksheet.Cells[1, 5].Value = "廠牌/型式";
         worksheet.Cells[1, 6].Value = "最低使用年限";
         worksheet.Cells[1, 7].Value = "已使用年月";
         worksheet.Cells[1, 8].Value = "廠牌已達年限";

         int row = 2;
         foreach (var item in properties)
         {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = item.Code;
            worksheet.Cells[row, 3].Value = item.Num;
            worksheet.Cells[row, 4].Value = item.Name;
            worksheet.Cells[row, 5].Value = item.Brand;
            worksheet.Cells[row, 6].Value = item.MinUse;
            worksheet.Cells[row, 7].Value = item.Used;
            worksheet.Cells[row, 8].Value = item.Over ? "是" : "";
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content


         // Save the Excel package to a file
         using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{filePath}' created successfully!");
      }
   }
   public static List<Property> GetTrashProperties(string filePath, PropType type)
   {
      var properties = new List<Property>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[type.Code];
         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }
         int rowCount = worksheet.Dimension.Rows; // Total rows
         for (int row = 2; row <= rowCount; row++)
         {
            var prop = new Property()
            {
               Code = worksheet.Cells[row, 2].Text,
               Name = worksheet.Cells[row, 4].Text,
               Brand = worksheet.Cells[row, 5].Text,
               Ps = worksheet.Cells[row, 6].Text
            };
            properties.Add(prop);
         }
      }
      return properties;
   }
   public static void Output(List<Property> properties, string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("sheet1");

         // Add headers
         worksheet.Cells[1, 1].Value = "";
         worksheet.Cells[1, 2].Value = "財產編號";
         worksheet.Cells[1, 3].Value = "名稱";
         worksheet.Cells[1, 4].Value = "別名";
         worksheet.Cells[1, 5].Value = "已達年限";
         worksheet.Cells[1, 6].Value = "廠牌";
         worksheet.Cells[1, 7].Value = "型式";
         worksheet.Cells[1, 8].Value = "保管單位";
         worksheet.Cells[1, 9].Value = "存置地點";

         int row = 2;
         foreach (var item in properties)
         {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = item.Num;
            worksheet.Cells[row, 3].Value = item.Name;
            worksheet.Cells[row, 4].Value = item.Title;
            worksheet.Cells[row, 5].Value = item.Over ? "是" : "";
            worksheet.Cells[row, 6].Value = item.Brand;
            worksheet.Cells[row, 7].Value = item.Type;
            worksheet.Cells[row, 8].Value = "資訊室";
            worksheet.Cells[row, 9].Value = UnitText(item.Location);
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content


         // Save the Excel package to a file
         using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{filePath}' created successfully!");
      }
   }
   static string UnitText(string code)
   {
      if (string.IsNullOrEmpty(code)) return "";
      code = code.Trim();
      if (code == "621") return "資訊室";
      if (code == "623") return "資訊室庫房";
      if (code == "801") return "電腦教室";
      return "";
   }
   public static void GetOutput(List<Property> properties, string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("sheet1");

         // Add headers
         worksheet.Cells[1, 1].Value = "";
         worksheet.Cells[1, 2].Value = "設備編號";
         worksheet.Cells[1, 3].Value = "名稱";
         worksheet.Cells[1, 4].Value = "廠牌/型式";
         worksheet.Cells[1, 5].Value = "報廢單據";

         int row = 2;
         foreach (var item in properties)
         {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = item.Code;
            worksheet.Cells[row, 3].Value = item.Name;
            worksheet.Cells[row, 4].Value = item.Brand;
            worksheet.Cells[row, 5].Value = item.Ps;
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content


         // Save the Excel package to a file
         using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{filePath}' created successfully!");
      }
   }
   public static List<PropType> GetPropTypes()
   {
      return new List<PropType>()
      {
         new PropType { Title = "印表機", Code = "TT" },
         new PropType { Title = "液晶螢幕", Code = "L" },
         new PropType { Title = "個人電腦", Code = "PP" },
         new PropType { Title = "不斷電系統", Code = "UP" },
         new PropType { Title = "其他", Code = "Other" }
      };
   }
   public static List<Property> GetPropertiesFromFile(string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      var properties = new List<Property>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[0];
         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }
         int rowCount = worksheet.Dimension.Rows; // Total rows
         for (int row = 2; row <= rowCount; row++)
         {
            var prop = new Property()
            {
               Num = worksheet.Cells[row, 1].Text.Trim(),
               MinUse = worksheet.Cells[row, 4].Text,
               Used = worksheet.Cells[row, 5].Text,
               Over = worksheet.Cells[row, 6].Text.Trim() == "是"
            };
            properties.Add(prop);
         }
      }
      return properties;
   }
   public static List<Trash> ReadTrashLcd(string filePath)
   {
      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var list = new List<Trash>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets["0210"];

         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }

         int rowCount = worksheet.Dimension.Rows; // Total rows

         // Iterate through the rows of the specified column
         for (int row = 2; row <= 100; row++)
         {
            string strNum = worksheet.Cells[row, 2].Text;
            if (string.IsNullOrEmpty(strNum)) continue;
            list.Add(new Trash
            {
               Num = worksheet.Cells[row, 2].Text,
               Name = worksheet.Cells[row, 4].Text,
               Brand = worksheet.Cells[row, 5].Text,
               PS = worksheet.Cells[row, 6].Text
            });
         }
      }
      if (list.GroupBy(x => x.Num).Any(group => group.Count() > 1)) throw new Exception("Has Duplicate Nums");
      return list.OrderBy(x => x.GetNumber).ToList();
   }
}
