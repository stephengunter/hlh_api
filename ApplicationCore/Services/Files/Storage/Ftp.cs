using ApplicationCore.Exceptions;
using FluentFTP;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Services.Files;
public class FtpStoragesService : IFileStoragesService
{
   private readonly FtpClient _client;
   private readonly string _root_directory;
   public FtpStoragesService(string host, string username, string pw, string directory)
   {
      _client = new FtpClient(host, username, pw);
      _root_directory = directory;
      Host = host;
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

   public string Host { get; private set; }

   string GetFolderPath(string folderPath) => CombinePath(_root_directory, folderPath);
   string CombinePath(string path1, string path2) => Path.Combine(path1, path2).Replace('\\', '/');

   public string Create(IFormFile file, string folderPath, string fileName, bool overwrite = false)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = CombinePath(folderPath, fileName);
      if (_client.DirectoryExists(folderPath))
      {
         if (_client.FileExists(filePath))
         {
            if (overwrite)
            {
               _client.UploadStream(file.OpenReadStream(), filePath, FtpRemoteExists.Overwrite, true);
            }
            else
            {
               filePath = CombinePath(folderPath, GetUniqueFileName(folderPath, fileName));
               _client.UploadStream(file.OpenReadStream(), filePath);
            }
         }
         else _client.UploadStream(file.OpenReadStream(), filePath);
      }
      else
      {
         _client.CreateDirectory(folderPath);
         _client.UploadStream(file.OpenReadStream(), filePath);
      }
      return filePath;
   }
   public string Create(byte[] filebytes, string folderPath, string fileName, bool overwrite = false)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = CombinePath(folderPath, fileName);

      if (_client.FileExists(filePath) && !overwrite)
      {
         fileName = GetUniqueFileName(folderPath, fileName);
         filePath = CombinePath(folderPath, fileName);
      }

      using (var stream = new MemoryStream(filebytes))
      {
         _client.UploadStream(stream, filePath, overwrite ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip, true);
      }

      return filePath;


      //if (_client.DirectoryExists(folderPath))
      //{
      //   if (_client.FileExists(filePath))
      //   {
      //      if (overwrite)
      //      {
      //         _client.UploadStream(fileStream, filePath, FtpRemoteExists.Overwrite, true);
      //      }
      //      else
      //      {
      //         filePath = CombinePath(folderPath, GetUniqueFileName(folderPath, fileName));
      //         _client.UploadStream(fileStream, filePath);
      //      }
      //   }
      //   else _client.UploadStream(fileStream, filePath);
      //}
      //else
      //{
      //   _client.CreateDirectory(folderPath);
      //   _client.UploadStream(fileStream, filePath);
      //}
      //return filePath;
   }
   public string Create(Stream stream, string folderPath, string fileName, bool overwrite = false)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = CombinePath(folderPath, fileName);

      if (_client.DirectoryExists(folderPath))
      {
         if (overwrite || !_client.FileExists(filePath))
         {
            _client.UploadStream(stream, filePath, overwrite ? FtpRemoteExists.Overwrite : FtpRemoteExists.NoCheck);
         }
         else
         {
            fileName = GetUniqueFileName(folderPath, fileName);
            filePath = CombinePath(folderPath, fileName);
            _client.UploadStream(stream, filePath, FtpRemoteExists.NoCheck);
         }
      }
      else
      {
         _client.CreateDirectory(folderPath);
         _client.UploadStream(stream, filePath, FtpRemoteExists.NoCheck);
      }
      return filePath;
   }

   public byte[] GetBytes(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = CombinePath(folderPath, fileName);
      if (_client.FileExists(filePath))
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
   public long GetFileSize(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = CombinePath(folderPath, fileName);
      if (_client.FileExists(filePath))
      {
         return _client.GetFileSize(filePath);
      }
      else throw new FileNotExistException(filePath);
   }
   public bool FileExists(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = CombinePath(folderPath, fileName);
      return _client.FileExists(filePath);
   }

   public string Move(string sourceFolder, string sourceFileName, string destFolder, string destFileName, bool overwrite = false)
   {
      sourceFolder = GetFolderPath(sourceFolder);
      string sourcePath = CombinePath(sourceFolder, sourceFileName);
      if (_client.FileExists(sourcePath))
      {
         destFolder = GetFolderPath(destFolder);
         if (!_client.DirectoryExists(destFolder))
         {
            if (!_client.CreateDirectory(destFolder)) throw new Exception($"CreateDirectory Failed. destFolder {destFolder}");
         }

         destFileName = FilesHelpers.GetUniqueFileName(destFolder, destFileName);
         string destPath = CombinePath(destFolder, destFileName);

         if (_client.MoveFile(sourcePath, destPath)) return destPath;
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

   public IEnumerable<string> GetFiles(string folderPath)
   {
      folderPath = GetFolderPath(folderPath);
      try
      {
         // Get the listing of the directory
         var items = _client.GetListing(folderPath);

         // Filter out directories and return only files
         var files = items.Where(item => item.Type == FtpObjectType.File)
                          .Select(item => item.FullName);
         return files;
      }
      catch (Exception ex)
      {
         // Log or handle the exception
         Console.WriteLine($"Error getting files from FTP: {ex.Message}");
         return Enumerable.Empty<string>();
      }
   }

   public IEnumerable<string> GetDirectories(string folderPath)
   {
      folderPath = GetFolderPath(folderPath);
      return _client.GetNameListing(folderPath).Where(item => _client.DirectoryExists(item));
   }
   public DateTime GetLastWriteTime(string folderPath, string fileName)
   {
      folderPath = GetFolderPath(folderPath);
      string filePath = CombinePath(folderPath, fileName);
      var fileItem = _client.GetObjectInfo(filePath);
      return fileItem != null ? fileItem.Modified : DateTime.MinValue;
   }
   public void CreateDirectory(string folderPath)
   {
      folderPath = GetFolderPath(folderPath);
      if (!_client.DirectoryExists(folderPath))
      {
         _client.CreateDirectory(folderPath);
      }
   }

   public void CopyFile(string sourceFilePath, string destFilePath, bool overwrite)
   {
      if (_client.FileExists(sourceFilePath))
      {
         _client.UploadFile(sourceFilePath, destFilePath, overwrite ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip);
      }
   }
   public void DeleteFile(string filePath)
   {
      if (_client.FileExists(filePath))
      {
         _client.DeleteFile(filePath);
      }
   }
   public void DeleteDirectory(string folderPath)
   {
      if (_client.DirectoryExists(folderPath))
      {
         _client.DeleteDirectory(folderPath, FtpListOption.AllFiles);
      }
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
