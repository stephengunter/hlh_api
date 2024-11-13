namespace ApplicationCore.Settings;

public class DbSettings
{
	public string Provider { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;
   public string Host { get; set; } = string.Empty;
   public string Username { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
   public bool CanTest { get; set; }
   public DbFullBackupSettings? FullBackupSettings { get; set; }
   public DbDifferentialBackupSettings? DifferentialBackupSettings { get; set; }
}

public class DbFullBackupSettings
{
   public string StartAt { get; set; } = string.Empty;
   public int GetStartAtTime()
   {
      return 1;
   }
}
public class DbDifferentialBackupSettings
{
   public string StartAt { get; set; } = string.Empty;
   public int IntervalHours { get; set; }
}