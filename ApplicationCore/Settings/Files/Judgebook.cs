namespace ApplicationCore.Settings.Files;
public class JudgebookFileSettings
{
   public bool NoSameCaseEntries { get; set; }


   //NAS
   public string Host { get; set; } = string.Empty;
   public string Directory { get; set; } = string.Empty;
   public string UserName { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
}



