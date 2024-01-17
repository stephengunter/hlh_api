namespace Web.Models;

public class SetPasswordRequest
{
   public string UserId { get; set; } = String.Empty;
   public string Password { get; set; } = String.Empty;
   public string Token { get; set; } = String.Empty;
}