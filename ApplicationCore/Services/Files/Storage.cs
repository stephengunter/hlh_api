using ApplicationCore.Exceptions;
using Ardalis.Specification;
using FluentFTP;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ApplicationCore.Services.Files;

public interface IFileStoragesService : IDisposable
{
   string Create(IFormFile file, string folderPath, string fileName);
   byte[] GetBytes(string folderPath, string fileName);
   // move the uploaded file
   string Move(string sourceFolder, string sourceFileName, string destFolder, string destFileName);

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
         
         //client.DownloadFile(@"C:\MyVideo_2.mp4", "/htdocs/MyVideo_2.mp4");
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
   string GetFolderPath(string folderPath) => CombinePath(_root_directory, folderPath);
   string CombinePath(string path1, string path2) => Path.Combine(path1, path2).Replace('\\', '/');

   public string Create(IFormFile file, string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = "";
      if (_client.DirectoryExists(folderPath))
      {
         filePath = CombinePath(folderPath, GetUniqueFileName(folderPath, fileName));
         _client.UploadStream(file.OpenReadStream(), filePath);

      }
      else
      {
         filePath = CombinePath(folderPath, fileName);
         _client.UploadStream(file.OpenReadStream(), filePath, FtpRemoteExists.Overwrite, true);
      }
      return filePath;
   }
   public byte[] GetBytes(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = CombinePath(folderPath, fileName);
      if(_client.FileExists(filePath))
      {
         byte[] bytes;
         if (!_client.DownloadBytes(out bytes, filePath))
         {
            throw new Exception($"DownloadBytes failed. path: {filePath}");
         }
         return bytes;
      }
      else throw new FileNotExistException(filePath);
   }

   public string Move(string sourceFolder, string sourceFileName, string destFolder, string destFileName)
   {
      sourceFolder = GetFolderPath(sourceFolder);
      string sourcePath = CombinePath(sourceFolder, sourceFileName);
      if (_client.FileExists(sourcePath))
      {
         destFolder = GetFolderPath(destFolder);
         if (!_client.DirectoryExists(destFolder))
         { 
            if(!_client.CreateDirectory(destFolder)) throw new Exception($"CreateDirectory Failed. destFolder {destFolder}");
         } 

         destFileName = FilesHelpers.GetUniqueFileName(destFolder, destFileName);
         string destPath = CombinePath(destFolder, destFileName);
         
         if(_client.MoveFile(sourcePath, destPath)) return destPath;
         throw new MoveFileFailedException(sourcePath, destPath);

      }
      else throw new FileNotExistException(sourcePath);
   }
   string GetUniqueFileName(string folderPath, string fileName)
   {
      string extension = Path.GetExtension(fileName);
      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
      string filePath = CombinePath(folderPath, fileName);
      int count = 1;
     
      while (_client.FileExists(filePath))
      {
         fileName = $"{fileNameWithoutExtension}_({count}){extension}";
         filePath = CombinePath(folderPath, fileName);
         count++;
      }
      return fileName;

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

   string GetFolderPath(string folderPath) => Path.Combine(_root_directory, folderPath);
   public string Create(IFormFile file, string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

      fileName = FilesHelpers.GetUniqueFileName(folderPath, fileName);
      string filePath = Path.Combine(folderPath, fileName);
      using (var stream = new FileStream(filePath, FileMode.Create))
      {
         file.CopyTo(stream);
      }
      return filePath;
   }

   public byte[] GetBytes(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = Path.Combine(folderPath, fileName);
      if (File.Exists(filePath))
      {
         return File.ReadAllBytes(filePath);
      }
      else throw new FileNotExistException(filePath);
   }
   public string Move(string sourceFolder, string sourceFileName, string destFolder, string destFileName)
   {
      
      string sourcePath = Path.Combine(sourceFolder, sourceFileName);
      if (File.Exists(sourcePath))
      {
         destFolder = GetFolderPath(destFolder);
         if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

         destFileName = FilesHelpers.GetUniqueFileName(destFolder, destFileName);
         string destPath = Path.Combine(destFolder, destFileName);
         File.Move(sourcePath, destPath);
         return destPath;
      }
      else throw new FileNotExistException(sourcePath);
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
