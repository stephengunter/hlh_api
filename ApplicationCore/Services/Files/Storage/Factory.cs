using ApplicationCore.Settings;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Services.Files;

public interface IFileStoragesService : IDisposable
{
   string Host { get; }
   string Create(IFormFile file, string folderPath, string fileName, bool overwrite = false);
   string Create(byte[] filebytes, string folderPath, string fileName, bool overwrite = false);
   string Create(Stream stream, string folderPath, string fileName, bool overwrite = false);
   byte[] GetBytes(string folderPath, string fileName);
   long GetFileSize(string folderPath, string fileName);
   string Move(string sourceFolder, string sourceFileName, string destFolder, string destFileName, bool overwrite = false);
   IEnumerable<string> GetFiles(string folderPath);
   IEnumerable<string> GetDirectories(string folderPath);
   DateTime GetLastWriteTime(string folderPath, string fileName);
   bool FileExists(string folderPath, string fileName);
   void CreateDirectory(string folderPath);
   void CopyFile(string sourceFilePath, string destFilePath, bool overwrite = false);
   void DeleteFile(string filePath);
   void DeleteDirectory(string folderPath);
}

public class FileStoragesServiceFactory()
{
   public IFileStoragesService Create(FileStorageSettings settings)
   {
      if (String.IsNullOrEmpty(settings.Host)) return new LocalStoragesService(settings.Directory);
      return new FtpStoragesService(settings.Host, settings.UserName,
            settings.Password, settings.Directory);
   }
}
