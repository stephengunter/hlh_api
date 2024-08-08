namespace ApplicationCore.Settings;
public class FileStorageSettings
{
   //NAS
   public string Host { get; set; } = string.Empty;
   public string Directory { get; set; } = string.Empty;
   public string UserName { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
}
public class FileBackupSettings
{
   public FileStorageSettings Source { get; set; }

   public FileStorageSettings Destination { get; set; }

   public bool CheckDirty { get; set; }
}



