namespace ApplicationCore.Settings;
public class LdapSettings
{
   public string Server { get; set; } = string.Empty;
   public int Port { get; set; }
   public string BaseDn { get; set; } = string.Empty;
   public string AdminUser { get; set; } = string.Empty;
   public string AdminPassword { get; set; } = string.Empty;
}



