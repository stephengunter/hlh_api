namespace Web.Models;
public class AuthResponse
{
	public AuthResponse(string token, int expiresIn, string refreshToken)
	{
		Token = token;
		ExpiresIn = expiresIn;
		RefreshToken = refreshToken;
	}
	public string Token { get; }
	public int ExpiresIn { get; }
	public string RefreshToken { get; set; }

}
public class AuthTokenResponse
{
   public AuthTokenResponse(string userName, string token, string url)
   {
		UserName = userName;
      Token = token;
      Url = url;
   }
   public string UserName { get; set; }
   public string Token { get; }
   public string Url { get; set; }

}