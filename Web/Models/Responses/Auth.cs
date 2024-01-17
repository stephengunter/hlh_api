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