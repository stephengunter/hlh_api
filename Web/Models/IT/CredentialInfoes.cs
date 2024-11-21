namespace Web.Models.IT;

public class CredentialInfoLabels
{
   public string Title => "¦WºÙ";
   public string Key => "Key";
   public string Username => "Username";
   public string Password => "±K½X"; 
   public string Ps => "³Æµù";
}

public class CredentialInfosFetchRequest
{
   public CredentialInfosFetchRequest(int page, int pageSize)
   {
      Page = page;
      PageSize = pageSize;
   }
   
   public int Page { get; set; }
   public int PageSize { get; set; }
}

public abstract class CredentialInfoBaseForm
{
   public string EntityType { get; set; } = String.Empty;
   public int EntityId { get; set; }
   public string Username { get; set; } = String.Empty;
   public string Password { get; set; } = string.Empty;

   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Ps { get; set; } = String.Empty;
}
public class CredentialInfoAddForm : CredentialInfoBaseForm
{
}
public class CredentialInfoEditForm : CredentialInfoBaseForm
{
   public int Id { get; set; }
   public bool CanRemove { get; set; }
}
public class CredentialInfoEditPasswordForm
{
   public int Id { get; set; }
   public string Username { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
}