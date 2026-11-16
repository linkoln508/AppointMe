using AppointMe.Api.Models;

namespace AppointMe.Api.Auth;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(ApplicationUser user, IList<string> roles);
}