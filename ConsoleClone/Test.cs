using ApplicationCore.Services.Files;
using ApplicationCore.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ConsoleClone;

public class Test
{
   private readonly IFileStoragesService _sourceFileService;
   private readonly IFileStoragesService _destinationFileService;
   private readonly FileBackupSettings _backupSettings;
   public Test(IOptions<FileBackupSettings> backupSettings, FileStoragesServiceFactory fileStoragesServiceFactory)
   {
      _backupSettings = backupSettings.Value;
      _sourceFileService = fileStoragesServiceFactory.Create(_backupSettings.Source);
      _destinationFileService = fileStoragesServiceFactory.Create(_backupSettings.Destination);
   }

   public async Task RunRun()
   {
      string localFilePath = @"c:\\temp\\test.txt";
      string sourceFolder = _backupSettings.Source.Directory;
      string destFolder = _backupSettings.Destination.Directory;

      _destinationFileService.CreateDirectory(destFolder);
      

      // Read the local file as a stream
      //using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
      //{
      //   try
      //   {
      //      // Call the Create method to upload the file
      //      string uploadedFilePath = _destinationFileService.Create(fileStream, "", Path.GetFileName(localFilePath));

      //      Console.WriteLine($"File uploaded successfully to: {uploadedFilePath}");
      //   }
      //   catch (Exception ex)
      //   {
      //      Console.WriteLine($"An error occurred while uploading the file: {ex.Message}");
      //   }
      //}
   }


   public async Task Run()
   {
      string sourceFolder = _backupSettings.Source.Directory;
      string destFolder = _backupSettings.Destination.Directory;
      try
      {
         CloneDirectory(sourceFolder, destFolder);
         CleanDestinationFolder(sourceFolder, destFolder);

         bool fileCountsAndSizesEqual = CheckFileCountsAndSizes(sourceFolder, destFolder);

         if (fileCountsAndSizesEqual)
         {
            LogInfo();
         }
         else
         {
            LogInfo("Backup completed, but the file counts or sizes do not match.");
         }
      }
      catch (Exception ex)
      {
         LogInfo($"An error occurred: {ex.Message}");
      }
      
   }

   void LogInfo(string msg = "")
   {
      if (String.IsNullOrEmpty(msg))
      {
         Console.WriteLine("backup success.");
      }
      else
      {
         Console.WriteLine(msg);
      }
      Console.ReadLine();
   }

   void CloneDirectory(string sourceDir, string destDir)
   {
      bool overwrite = true;
      // Create destination directory if it doesn't exist
      _destinationFileService.CreateDirectory(destDir);

      var sourceFilePathList = _sourceFileService.GetFiles(sourceDir);
      // Get all files in the source directory
      foreach (string sourceFilePath in sourceFilePathList)
      {
         // Determine the destination file path
         string filename = Path.GetFileName(sourceFilePath);
         string destFilePath = Path.Combine(destDir, filename);

         var destFilePathList = _destinationFileService.GetFiles(destDir);

         // Check if the file exists in the destination directory
         if (_destinationFileService.GetFiles(destDir).Contains(destFilePath))
         {
            if (_backupSettings.CheckDirty)
            {
               // Check if the file is dirty (has been modified since the last backup)
               DateTime sourceLastModified = _sourceFileService.GetLastWriteTime(sourceDir, filename);
               DateTime destLastModified = _destinationFileService.GetLastWriteTime(destDir, filename);

               if (sourceLastModified > destLastModified)
               {
                  // Copy the file
                  var filebytes = _sourceFileService.GetBytes(sourceDir, filename);

                  _destinationFileService.Create(filebytes, destDir, filename, overwrite);

                  Console.WriteLine($"Updated: {sourceFilePath}");
               }
            }
            else
            {
               // Copy the file
               var filebytes = _sourceFileService.GetBytes(sourceDir, filename);

               _destinationFileService.Create(filebytes, destDir, filename, overwrite);

               Console.WriteLine($"Updated: {sourceFilePath}");
            }
            
         }
         else
         {
            // Copy the file

            var filebytes = _sourceFileService.GetBytes(sourceDir, filename);

            _destinationFileService.Create(filebytes, destDir, filename, overwrite);

            Console.WriteLine($"Copied: {sourceFilePath}");
         }
      }

      // Recursively clone subdirectories
      foreach (string subDir in _sourceFileService.GetDirectories(sourceDir))
      {
         string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
         CloneDirectory(subDir, destSubDir);
      }
   }
   void CleanDestinationFolder(string sourceDir, string destDir)
   {
      // Get all files in the destination directory
      foreach (string filepath in _destinationFileService.GetFiles(destDir))
      {
         // Determine the source file path
         string sourceFilepath = Path.Combine(sourceDir, Path.GetFileName(filepath));

         // Check if the file exists in the source directory
         if (!_sourceFileService.GetFiles(sourceDir).Contains(sourceFilepath))
         {
            // Delete the file from the destination directory
            _destinationFileService.DeleteFile(filepath);
            Console.WriteLine($"Deleted: {filepath}");
         }
      }

      // Recursively clean subdirectories
      foreach (string subDir in _destinationFileService.GetDirectories(destDir))
      {
         string sourceSubDir = Path.Combine(sourceDir, Path.GetFileName(subDir));
         if (_sourceFileService.GetDirectories(sourceDir).Contains(sourceSubDir))
         {
            CleanDestinationFolder(sourceSubDir, subDir);
         }
         else
         {
            _destinationFileService.DeleteDirectory(subDir);
            Console.WriteLine($"Deleted directory: {subDir}");
         }
      }
   }


   bool CheckFileCountsAndSizes(string sourceDir, string destDir)
   {
      // AreFolderFileCountsAndSizesEqual
      // Get the total number of files and total size in the source directory
      var sourceStats = GetTotalFileCountAndSize(sourceDir, _sourceFileService);

      // Get the total number of files and total size in the destination directory
      var destStats = GetTotalFileCountAndSize(destDir, _destinationFileService);

      // Compare the file counts and sizes
      return sourceStats.fileCount == destStats.fileCount && sourceStats.totalSize == destStats.totalSize;
   }

   static (int fileCount, long totalSize) GetTotalFileCountAndSize(string dir, IFileStoragesService storageService)
   {
      int fileCount = 0;
      long totalSize = 0;

      foreach (string file in storageService.GetFiles(dir))
      {
         fileCount++;
         totalSize += storageService.GetFileSize(dir, file);
      }

      // Recursively count files and their sizes in subdirectories
      foreach (string subDir in storageService.GetDirectories(dir))
      {
         var subDirStats = GetTotalFileCountAndSize(subDir, storageService);
         fileCount += subDirStats.fileCount;
         totalSize += subDirStats.totalSize;
      }

      return (fileCount, totalSize);
   }


}
