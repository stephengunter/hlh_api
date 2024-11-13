using Microsoft.AspNetCore.Mvc;
using UtfUnknown;
using ApplicationCore.Views.AD;
using OfficeOpenXml;
using System.Text;
using Infrastructure.Helpers;

namespace Web.Controllers.Open;

public class CsvController : BaseOpenController
{
	public CsvController()
	{
      
	}

	[HttpPost]
	public async Task<ActionResult<AdUser?>> Convert(IFormFile file)
	{
      if (file == null || file.Length == 0)
      {
         return BadRequest("file not valid.");
      }
      string fileExtension = Path.GetExtension(file.FileName);
      bool isXml = fileExtension.EqualTo(".xlsx") || fileExtension.EqualTo(".xls");
      if (isXml)
      {
         using (var stream = new MemoryStream())
         {
            await file.CopyToAsync(stream);
            stream.Position = 0; // Reset stream position

            var csvStream = ConvertExcelToCsv(stream);
            csvStream.Position = 0; // Reset stream position

            // Return the CSV file as a File result
            return File(csvStream, "text/csv", Path.GetFileNameWithoutExtension(file.FileName) + ".csv");
         }
      }
      bool isCsv = fileExtension.EqualTo(".csv");
      if(!isCsv) return BadRequest("file not valid.");
      using (var stream = new MemoryStream())
      {
         await file.CopyToAsync(stream);
         stream.Position = 0; // Reset stream position

         var utf8Stream = await ConvertCsvToUtf8Async(stream);
         return File(utf8Stream, "text/csv", Path.GetFileNameWithoutExtension(file.FileName) + "_utf8.csv");
      }
   }

   MemoryStream ConvertExcelToCsv(Stream excelFileStream)
   {
      // Set EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      using (var package = new ExcelPackage(excelFileStream))
      {
         var csvContent = new StringBuilder();

         // Process the first worksheet in the workbook
         var worksheet = package.Workbook.Worksheets[0];
         int rowCount = worksheet.Dimension.Rows;
         int colCount = worksheet.Dimension.Columns;

         // Loop through each row and column, building CSV content
         for (int row = 1; row <= rowCount; row++)
         {
            var rowValues = new string[colCount];
            for (int col = 1; col <= colCount; col++)
            {
               rowValues[col - 1] = worksheet.Cells[row, col].Text.Replace(",", ""); // Remove commas for CSV format
            }
            csvContent.AppendLine(string.Join(",", rowValues));
         }

         // Convert CSV content to a MemoryStream with UTF-8 encoding
         var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent.ToString()));
         return csvStream;
      }
   }
   async Task<MemoryStream> ConvertCsvToUtf8Async(Stream csvStream)
   {
      csvStream.Position = 0;

      // Read raw bytes from the stream
      byte[] fileBytes;
      using (var memoryStream = new MemoryStream())
      {
         await csvStream.CopyToAsync(memoryStream);
         fileBytes = memoryStream.ToArray();
      }

      // Detect encoding using UtfUnknown
      var result = CharsetDetector.DetectFromBytes(fileBytes);
      var detectedEncoding = result.Detected?.Encoding ?? Encoding.UTF8; // Default to UTF-8 if detection fails

      // Convert the content to UTF-8
      string content = detectedEncoding.GetString(fileBytes);  // Decode bytes to string using detected encoding
      byte[] utf8Bytes = Encoding.UTF8.GetBytes(content);      // Re-encode to UTF-8 bytes
      var utf8Stream = new MemoryStream(utf8Bytes);

      // Reset the position of the stream to the beginning for returning
      utf8Stream.Position = 0;
      return utf8Stream;
   }

}
