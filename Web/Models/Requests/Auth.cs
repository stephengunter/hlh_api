using System.ComponentModel.DataAnnotations;

namespace Web.Models;
public class RefreshTokenRequest
{
   public string AccessToken { get; set; } = String.Empty;
   public string RefreshToken { get; set; } = String.Empty;

}

public class LoginRequest
{
   [Required(ErrorMessage = "必須填寫使用者名稱")]
   public string Username { get; set; } = String.Empty;

   [Required(ErrorMessage = "必須填寫密碼")]
   [DataType(DataType.Password)]
   public string Password { get; set; } = String.Empty;
}

public class OAuthLoginRequest
{
   public string Token { get; set; } = String.Empty;

}

public class AuthTokenRequest
{
   public string Key { get; set; } = String.Empty;
   public string UserName { get; set; } = String.Empty;

   public string AdListJson { get; set; } = String.Empty;

   public string Provider { get; set; } = String.Empty;

}

public class AuthTokenLoginRequest
{
   public string Token { get; set; } = String.Empty;
   public string UserName { get; set; } = String.Empty;
   public string Provider { get; set; } = String.Empty;

}