using System;

namespace Infrastructure.Helpers;
public static class FilesHelpers
{
   public static string GetUniqueFileName(string folderPath, string fileName)
   {
      string extension = Path.GetExtension(fileName);
      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
      string filePath = Path.Combine(folderPath, fileName);
      int count = 1;
      while (File.Exists(filePath))
      {
         fileName = $"{fileNameWithoutExtension}_({count}){extension}";
         filePath = Path.Combine(folderPath, fileName);
         count++;
      }
      return fileName;

   }
}
