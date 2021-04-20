using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string CreateJWTToken(AppUser user);
    }
}