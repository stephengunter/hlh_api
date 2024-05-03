namespace ApplicationCore.Settings;
public class JudSettings
{
	public string Domain { get; set; } = string.Empty;
	public string Intra { get; set; } = string.Empty;
}

public class Jud3Settings
{
   public string IP { get; set; } = string.Empty;
   public string Key { get; set; } = string.Empty;

   public int TokenValidMinutes { get; set; }
}



