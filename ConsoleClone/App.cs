using ApplicationCore.Consts;
using ApplicationCore.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClone;

public class App
{
   public async Task Run(IConfiguration config)
   {
      var fileBackupSettings = config.GetSection(SettingsKeys.FileBackup).Get<FileBackupSettings>();
      string sourceFolder = fileBackupSettings!.SourceFolder;
      string destFolder = fileBackupSettings.DestFolder;
      try
      {
         CloneDirectory(sourceFolder, destFolder);
         CleanDestinationFolder(sourceFolder, destFolder);
         Console.WriteLine("Backup completed successfully.");
      }
      catch (Exception ex)
      {
         Console.WriteLine($"An error occurred: {ex.Message}");
      }
      Console.ReadLine();
   }

   static void CloneDirectory(string sourceDir, string destDir)
   {
      // Create destination directory if it doesn't exist
      Directory.CreateDirectory(destDir);

      // Get all files in the source directory
      foreach (string file in Directory.GetFiles(sourceDir))
      {
         // Determine the destination file path
         string destFile = Path.Combine(destDir, Path.GetFileName(file));

         // Check if the file exists in the destination directory
         if (File.Exists(destFile))
         {
            // Check if the file is dirty (has been modified since the last backup)
            DateTime sourceLastModified = File.GetLastWriteTime(file);
            DateTime destLastModified = File.GetLastWriteTime(destFile);

            if (sourceLastModified > destLastModified)
            {
               // Copy the file
               File.Copy(file, destFile, true);
               Console.WriteLine($"Updated: {file}");
            }
         }
         else
         {

            // Copy the file
            File.Copy(file, destFile);
            Console.WriteLine($"Copied: {file}");
         }
      }

      // Recursively clone subdirectories
      foreach (string subDir in Directory.GetDirectories(sourceDir))
      {
         string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
         CloneDirectory(subDir, destSubDir);
      }
   }

   static void CleanDestinationFolder(string sourceDir, string destDir)
   {
      // Get all files in the destination directory
      foreach (string file in Directory.GetFiles(destDir))
      {
         // Determine the source file path
         string sourceFile = Path.Combine(sourceDir, Path.GetFileName(file));

         // Check if the file exists in the source directory
         if (!File.Exists(sourceFile))
         {
            // Delete the file from the destination directory
            File.Delete(file);
            Console.WriteLine($"Deleted: {file}");
         }
      }

      // Recursively clean subdirectories
      foreach (string subDir in Directory.GetDirectories(destDir))
      {
         string sourceSubDir = Path.Combine(sourceDir, Path.GetFileName(subDir));
         if (Directory.Exists(sourceSubDir))
         {
            CleanDestinationFolder(sourceSubDir, subDir);
         }
         else
         {
            Directory.Delete(subDir, true);
            Console.WriteLine($"Deleted directory: {subDir}");
         }
      }
   }
}
