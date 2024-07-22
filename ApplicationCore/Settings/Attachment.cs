namespace ApplicationCore.Settings;
public class AttachmentSettings
{
   public int MaxFileSize { get; set; } // 100 = 100MB;

   //NAS
   public string Host { get; set; } = string.Empty;
   public string Directory { get; set; } = string.Empty;
   public string UserName { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
}



