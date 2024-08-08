using ApplicationCore.Exceptions;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Services.Files;
public class LocalStoragesService : IFileStoragesService
{
   private readonly string _root_directory;
   public LocalStoragesService(string directory)
   {
      _root_directory = directory;
      Host = "";
   }

   public string Host { get; private set; }

   string GetFolderPath(string folderPath) => Path.Combine(_root_directory, folderPath);
   public string Create(IFormFile file, string folderPath, string fileName, bool overwrite = false)
   {
      folderPath = GetFolderPath(folderPath);
      if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

      if (overwrite)
      {
         string filePath = Path.Combine(folderPath, fileName);
         using (var stream = new FileStream(filePath, FileMode.Create))
         {
            file.CopyTo(stream);
         }
         return filePath;
      }
      else
      {
         fileName = FilesHelpers.GetUniqueFileName(folderPath, fileName);
         string filePath = Path.Combine(folderPath, fileName);
         using (var stream = new FileStream(filePath, FileMode.CreateNew))
         {
            file.CopyTo(stream);
         }
         return filePath;
      }
   }
   public string Create(byte[] filebytes, string folderPath, string fileName, bool overwrite = false)
   {
      folderPath = GetFolderPath(folderPath);
      if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

      string filePath = Path.Combine(folderPath, fileName);

      if (File.Exists(filePath) && !overwrite)
      {
         fileName = FilesHelpers.GetUniqueFileName(folderPath, fileName);
         filePath = Path.Combine(folderPath, fileName);
      }

      using (var stream = new FileStream(filePath, FileMode.Create))
      {
         stream.Write(filebytes, 0, filebytes.Length);
      }
      return filePath;
   }
   public string Create(Stream stream, string folderPath, string fileName, bool overwrite = false)
   {
      folderPath = GetFolderPath(folderPath);
      if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

      string filePath = Path.Combine(folderPath, fileName);
      if (overwrite || !File.Exists(filePath))
      {
         using (var fileStream = new FileStream(filePath, FileMode.Create))
         {
            stream.CopyTo(fileStream);
         }
      }
      else
      {
         fileName = FilesHelpers.GetUniqueFileName(folderPath, fileName);
         filePath = Path.Combine(folderPath, fileName);
         using (var fileStream = new FileStream(filePath, FileMode.Create))
         {
            stream.CopyTo(fileStream);
         }
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
   public long GetFileSize(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = Path.Combine(folderPath, fileName);
      FileInfo fileInfo = new FileInfo(filePath);
      return fileInfo.Length;
   }

   public string Move(string sourceFolder, string sourceFileName, string destFolder, string destFileName, bool overwrite = false)
   {
      string sourcePath = Path.Combine(sourceFolder, sourceFileName);
      if (!File.Exists(sourcePath)) throw new FileNotExistException(sourcePath);
      
      destFolder = GetFolderPath(destFolder);
      if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

      if (overwrite)
      {
         string destPath = Path.Combine(destFolder, destFileName);
         File.Move(sourcePath, destPath, overwrite);
         return destPath;
      }
      else 
      {
         string destPath = Path.Combine(destFolder, FilesHelpers.GetUniqueFileName(destFolder, destFileName));
         File.Move(sourcePath, destPath);
         return destPath;
      }
   }
   public bool FileExists(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = Path.Combine(folderPath, fileName);
      return File.Exists(filePath);
   }
   public IEnumerable<string> GetFiles(string folderPath)
   {
      folderPath = GetFolderPath(folderPath);
      if (Directory.Exists(folderPath))
      {
         return Directory.GetFiles(folderPath);
      }
      else
      {
         throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");
      }
   }

   public IEnumerable<string> GetDirectories(string folderPath)
   {
      folderPath = GetFolderPath(folderPath);
      if (Directory.Exists(folderPath))
      {
         return Directory.GetDirectories(folderPath);
      }
      else
      {
         throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");
      }
   }

   public DateTime GetLastWriteTime(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = Path.Combine(folderPath, fileName);
      if (File.Exists(filePath))
      {
         return File.GetLastWriteTimeUtc(filePath);
      }
      else
      {
         throw new FileNotFoundException($"The file '{filePath}' does not exist.");
      }
   }

   public void CreateDirectory(string folderPath)
   {
      folderPath = GetFolderPath(folderPath);
      Directory.CreateDirectory(folderPath);
   }

   public void CopyFile(string sourceFilePath, string destFilePath, bool overwrite)
   {
      destFilePath = GetFolderPath(destFilePath);
      File.Copy(sourceFilePath, destFilePath, overwrite);
   }

   public void DeleteFile(string filePath)
   {
      if (File.Exists(filePath))
      {
         File.Delete(filePath);
      }
   }

   public void DeleteDirectory(string folderPath)
   {
      folderPath = GetFolderPath(folderPath);
      if (Directory.Exists(folderPath))
      {
         Directory.Delete(folderPath, true);
      }
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