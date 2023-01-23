using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityCore.Services.Contracts
{
    public interface IAccountContract
    {
         Task<IdentityResult> Register(RegisterEntity register);
         Task<List<UserEntity>> GetUserList();
        Task<bool> Login(LoginModel loginModel);
    }
}
