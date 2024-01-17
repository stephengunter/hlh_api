namespace ApplicationCore.Exceptions;

public class TokenResolveFailedException : Exception
{
	public TokenResolveFailedException(string msg = "") : base(msg)
	{

	}
}
public class RefreshTokenFailedException : Exception
{
	public RefreshTokenFailedException(string msg) : base(msg)
	{

	}
}
