using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Services.Contracts;
using IdentityCore.Utility;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityRole = Microsoft.AspNetCore.Identity.IdentityRole;

namespace IdentityCore.Services.Implementations
{
    public class RoleImplementation : IRoleContract
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserEntity> _userManager;

        public RoleImplementation(RoleManager<IdentityRole> roleManager, UserManager<UserEntity> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        //yet to be tested.
        public async Task<bool> AddClaimToRole(string roleName, string claimName)
        {
            try
            {
                claimName = claimName.Trim();
                IdentityRole role = new IdentityRole(roleName);
                
                IList<Claim> allClaims = await _roleManager.GetClaimsAsync(role);
                List<string> cName = new List<string>();
                foreach (var i in allClaims)
                {
                    cName.Add(i.Type.ToString());
                }
                bool check = cName.Contains(claimName);
                if (check)
                {             
                   //claim already exist for given role.
                    return false;
                }
                Claim claim = new Claim(claimName, "true");
                IdentityResult check1 = await _roleManager.AddClaimAsync(role, claim);
                if (check1.Succeeded)
                {
                    return true;
                }
                //failed in identity operation.
                return false;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //yet to be tested.
        public async Task<bool> RemoveClaimFromRole(string roleName, string claimName)
        {
            try
            {
                claimName = claimName.Trim();
                IdentityRole role = new IdentityRole(roleName);
                IList<Claim> allClaims = await _roleManager.GetClaimsAsync(role);
                List<string> cName = new List<string>();
                foreach (var i in allClaims)
                {
                    cName.Add(i.Type.ToString());
                }
                bool check = cName.Contains(claimName);
                            
                if (!check)
                {
                    //claim doesnot exist for given role.
                    return false;
                }
                Claim claim = new Claim(claimName, "true");
                IdentityResult check1 = await _roleManager.RemoveClaimAsync(role, claim);
                if (check1.Succeeded)
                {
                    return true;
                }
                //failed in identity operation.
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //yet to be tested.
        public async Task<IList<Claim>> GetClaimForRole(string roleName)
        {
            try
            {
                roleName = roleName.Trim();
                IdentityRole role = new IdentityRole(roleName);
                IList<Claim> allClaims = await _roleManager.GetClaimsAsync(role);
                return allClaims;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AssignRoleToUser(string userEmail, string roleName)
        {
            try
            {
                roleName = roleName.Trim();
                UserEntity user = await _userManager.FindByEmailAsync(userEmail);
                bool result = await _roleManager.RoleExistsAsync(roleName);
                if (result && user != null)
                {
                    IdentityResult result1 = await _userManager.AddToRoleAsync(user, roleName);
                    if (result1.Succeeded)
                    {
                        return true;
                    }

                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> CreateRole(string roleName)
        {
            try
            {
                roleName = roleName.Trim();
                bool check = await _roleManager.RoleExistsAsync(roleName);
                if (!check)
                {
                    IdentityRole role = new IdentityRole(roleName);
                    await _roleManager.CreateAsync(role);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //yet to be tested.
        public async Task<bool> DeleteRole(string roleName)
        {
            try
            {

                roleName = roleName.Trim();
                var role = _roleManager.Roles.Where(r => r.Name == roleName).FirstOrDefault();
                IdentityResult check = await _roleManager.DeleteAsync(role);
                if (check.Succeeded)
                {
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
       public async Task<bool> RemoveRoleFromUser(UserEntity user,string roleName)
        {
            try
            {
                roleName = roleName.Trim();
                IdentityResult check = await _userManager.RemoveFromRoleAsync(user,roleName);
                if (check.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<IdentityRole> GetRoles()
        {
            try
            {
                List<IdentityRole> list = _roleManager.Roles.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
