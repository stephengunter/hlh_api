using HtmlAgilityPack;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDev;
public class Card
{
   public string Item { get; set; } = string.Empty;
   public string ICCardNumber { get; set; } = string.Empty;
   public string CardType { get; set; } = string.Empty;
   public string CertificateDN { get; set; } = string.Empty;
   public string CertificateSerial { get; set; } = string.Empty;
   public string Status { get; set; } = string.Empty;
   public string IssueTime { get; set; } = string.Empty;
   public string ValidTime { get; set; } = string.Empty;
   public string IssueTimeString
      => string.IsNullOrEmpty(IssueTime) ? "" : $"{IssueTime.Substring(0, 4)}-{IssueTime.Substring(4, 2)}-{IssueTime.Substring(6, 2)}";
   public string ValidTimeString
       => string.IsNullOrEmpty(IssueTime) ? "" : $"{ValidTime.Substring(0, 4)}-{ValidTime.Substring(4, 2)}-{ValidTime.Substring(6, 2)}";

   public bool IsValid => Status == "有效";

   public override string ToString()
   {
      return $"Item: {Item}, IC Card: {ICCardNumber}, Card Type: {CardType}, " +
             $"DN: {CertificateDN}, Serial: {CertificateSerial}, Status: {Status}, " +
             $"Issued: {IssueTimeString}, Valid: {ValidTimeString}";
   }
}
public class CardHelpers
{
   public static void Test()
   {
      string filePath = @"C:\\temp\\202503\index.html"; // Path to your .html file
      if (File.Exists(filePath))
      {
         // Load the HTML document
         var htmlDoc = new HtmlDocument();
         htmlDoc.Load(filePath);

         // Get the root node
         var root = htmlDoc.DocumentNode;
         var tables = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'tab_info')]");
         List<Card> cards = new List<Card>();
         if (tables != null)
         {
            foreach (var table in tables)
            {
               var rows = table.SelectNodes(".//tr");
               var card = new Card();

               foreach (var row in rows)
               {
                  var columns = row.SelectNodes(".//th|.//td");

                  if (columns != null && columns.Count == 2)
                  {
                     string key = columns[0].InnerText.Trim();
                     string value = HtmlEntity.DeEntitize(columns[1].InnerText.Trim());

                     switch (key)
                     {
                        case "項目": card.Item = value; break;
                        case "IC卡號": card.ICCardNumber = value; break;
                        case "卡別": card.CardType = value; break;
                        case "憑證DN": card.CertificateDN = value; break;
                        case "憑證序號": card.CertificateSerial = value; break;
                        case "狀態": card.Status = value; break;
                        case "簽發時間": card.IssueTime = value; break;
                        case "有效時間": card.ValidTime = value; break;
                     }
                  }
               }
               var exist = cards.FirstOrDefault(x => x.ICCardNumber == card.ICCardNumber);
               if (exist == null) cards.Add(card);

            }

            OutputCards(cards.Where(x => x.IsValid).ToList(), @"C:\\temp\\202503\cards_valid.xlsx", "有效卡");
            OutputCards(cards.Where(x => !x.IsValid).ToList(), @"C:\\temp\\202503\cards_invalid.xlsx", "過期卡");
         }
         else
         {
            Console.WriteLine("No tables with class 'tab_info' found.");
         }

         Console.WriteLine("HTML file processed successfully.");
      }
      else
      {
         Console.WriteLine("File not found.");
      }

   }
   public static void OutputCards(List<Card> properties, string filePath, string sheetName)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);

         // Add headers
         worksheet.Cells[1, 1].Value = "序號";
         worksheet.Cells[1, 2].Value = "IC卡號";
         worksheet.Cells[1, 3].Value = "卡別";
         worksheet.Cells[1, 4].Value = "狀態";
         worksheet.Cells[1, 5].Value = "簽發日期";
         worksheet.Cells[1, 6].Value = "有效期限";

         int row = 2;
         foreach (var item in properties)
         {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = item.ICCardNumber;
            worksheet.Cells[row, 3].Value = item.CardType;
            worksheet.Cells[row, 4].Value = item.Status;
            worksheet.Cells[row, 5].Value = item.IssueTimeString;
            worksheet.Cells[row, 6].Value = item.ValidTimeString;
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 8].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content


         // Save the Excel package to a file
         using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{filePath}' created successfully!");
      }
   }
}