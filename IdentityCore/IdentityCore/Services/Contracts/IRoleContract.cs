using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityCore.Services.Contracts
{
    public interface IRoleContract
    {
        Task<bool> AssignRoleToUser(string userEmail, string roleName);
        Task<bool> CreateRole(string roleName);
        Task<bool> DeleteRole(string roleName);
        Task<bool> AddClaimToRole(string roleName,string claimName);
        List<IdentityRole> GetRoles();
        Task<bool> RemoveClaimFromRole(string roleName, string claimName);
        Task<IList<Claim>> GetClaimForRole(string roleName);
        Task<bool> RemoveRoleFromUser(UserEntity user, string roleName);
    }
}
