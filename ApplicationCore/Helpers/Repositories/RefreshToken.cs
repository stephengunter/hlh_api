using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Models.Auth;
using ApplicationCore.Specifications.Auth;

namespace ApplicationCore.Helpers;

public static class RefreshTokenRepositoryHelpers
{
	public static async Task<RefreshToken?> FindAsync(this IDefaultRepository<RefreshToken> refreshTokensRepository, User user)
		=> await refreshTokensRepository.FirstOrDefaultAsync(new RefreshTokensSpecification(user));

}