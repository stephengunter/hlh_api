using FluentFTP;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ApplicationCore.Services.Files;

public interface IFileStoragesService : IDisposable
{
   string Create(IFormFile file, string folderPath, string fileName);
   
}

public class FtpStoragesService : IFileStoragesService
{
   private readonly FtpClient _client;
   private readonly string _root_directory;
   public FtpStoragesService(string host, string username, string pw, string directory)
   {
      _client = new FtpClient(host, username, pw);
      _root_directory = directory;
      try
      {
         _client.Connect();

         if (!_client.IsConnected)
         {
            // Handle connection failure if needed
            throw new Exception("Failed to connect to the FTP server.");
         }
      }
      catch (Exception ex)
      {
         // Log or handle the connection error
         throw new Exception($"FTP Connection Error: {ex.Message}");
      }
   }
   string GetFolderPath(string folderPath) => Path.Combine(_root_directory, folderPath);

   public string Create(IFormFile file, string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      if (_client.DirectoryExists(folderPath))
      {
         string filePath = GetUniqueFileName(folderPath, fileName);

         using (var stream = new FileStream(filePath, FileMode.Create))
         {
            file.CopyTo(stream);
         }
         return filePath;


      }
      else
      {
         _client.UploadStream(file.OpenReadStream(), Path.Combine(folderPath, fileName), FtpRemoteExists.Overwrite, true);
         return Path.Combine(folderPath, fileName);
      }
      
   }
   string GetUniqueFileName(string folderPath, string fileName)
   {
      string extension = Path.GetExtension(fileName);
      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

      folderPath = GetFolderPath(folderPath);
      string filePath = Path.Combine(folderPath, fileName);
      int count = 1;
     
      while (_client.FileExists(filePath))
      {
         string newFileName = $"{fileNameWithoutExtension}_({count}){extension}";
         filePath = Path.Combine(folderPath, newFileName);
         count++;
      }
      return filePath;

   }

   public void Dispose()
   {
      Dispose(true);
      GC.SuppressFinalize(this);
   }

   protected virtual void Dispose(bool disposing)
   {
      if (disposing)
      {
         // Disconnect from the FTP server
         if (_client.IsConnected)
         {
            _client.Disconnect();
         }

         // Dispose managed resources
         _client.Dispose();
      }
      // Dispose unmanaged resources
   }
}

public class LocalStoragesService : IFileStoragesService
{
   private readonly string _root_directory;
   public LocalStoragesService(string directory)
   {
      _root_directory = directory;
   }
   public string Create(IFormFile file, string folderPath, string fileName)
   {
      string filePath = FilesHelpers.GetUniqueFileName(folderPath, fileName);

      using (var stream = new FileStream(filePath, FileMode.Create))
      {
         file.CopyTo(stream);
      }
      return filePath;
   }

   public void Dispose()
   {
      Dispose(true);
      GC.SuppressFinalize(this);
   }

   protected virtual void Dispose(bool disposing)
   {
      
      // Dispose unmanaged resources
   }
}
