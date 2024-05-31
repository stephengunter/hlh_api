namespace ApplicationCore.Settings.Files;
public class JudgebookFileSettings
{
   public bool AllowEmptyJudgeDate { get; set; }
   public bool AllowEmptyFileNumber { get; set; }
   public bool NoSameCaseEntries { get; set; }

   public string Title { get; set; } = string.Empty;

   //NAS
   public string Host { get; set; } = string.Empty;
   public string Directory { get; set; } = string.Empty;
   public string UserName { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
}



