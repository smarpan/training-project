using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Services.Contracts;
using IdentityCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace IdentityCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly IRoleContract _contract;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserEntity> _userManager;

        public RoleController(IRoleContract contract, 
            RoleManager<IdentityRole> roleManager,
            UserManager<UserEntity> userManager)
        {
            _contract = contract;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Route("createrole")]
        [HttpPost]
        [Authorize(Policy = "CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleModel roleName)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Could not create role" };
            try
            {

                bool check = await _roleManager.RoleExistsAsync(roleName.RoleName);
                if (check)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "Role already exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                bool result = await _contract.CreateRole(roleName.RoleName);
                if (result)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Role Created" };
                    return StatusCode(StatusCodes.Status200OK, response);

                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
       
        //yet to be tested.
        [Route("addclaimtorole")]
        [HttpPost]
        public async Task<IActionResult> AddClaimToRole([FromBody]ClaimRoleModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Could not add claim" };
            try
            {
                bool check = await _roleManager.RoleExistsAsync(model.RoleName);
                if (!check)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "Role does not exist exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                bool result = await _contract.AddClaimToRole(model.RoleName, model.ClaimName);
                if (result)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "Claim: "+model.ClaimName +" Added to "+model.RoleName };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        //yet to be tested.
        [Route("removeclaimfromrole")]
        [HttpPost]        
        public async Task<IActionResult> RemoveClaimFromRole([FromBody] ClaimRoleModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Could not remove claim from role" };  
            try
            {
                bool check = await _roleManager.RoleExistsAsync(model.RoleName);
                if (!check)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "Role does not exist exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                bool check1 = await _contract.RemoveClaimFromRole(model.RoleName,model.ClaimName);
                if (check1)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Claim removed successfully" };
                    return StatusCode(StatusCodes.Status200OK,response);
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };
                  return StatusCode(StatusCodes.Status500InternalServerError, response); 
            }
        }

        //yet to be tested.
        [Route("removeclaimfromrole/{roleName}")]
        [HttpGet]      
        public async Task<IActionResult> GetClaimForRole(string roleName)
        {
            ResponseApi<IList<Claim>> response = new ResponseApi<IList<Claim>>() { Status = false, Message = "could not get the roles" }; 
            try
            {
                bool check = await _roleManager.RoleExistsAsync(roleName);
                if (!check)
                {
                    response = new ResponseApi<IList<Claim>>() { Status = false, Message = "Role does not exist exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                IList<Claim> check1 = await _contract.GetClaimForRole(roleName);          
                    response = new ResponseApi<IList<Claim>>() { Status = true, Message = "Claims are here",Data=check1 };
                    return StatusCode(StatusCodes.Status200OK, response);
                
            }
            catch (Exception ex)
            {
                response = new ResponseApi<IList<Claim>>() { Status = true, Message = ex.Message };
                return StatusCode(StatusCodes.Status200OK, response);
            }
        }

        //yet to be tested.
        [Route("deleterole")]
        [HttpPost]
        [Authorize(Policy = "DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] CreateRoleModel roleModel)
        {

            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Could not delete role" };
            try
            {
                bool check = await _roleManager.RoleExistsAsync(roleModel.RoleName);
                if (!check)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "Role does not exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                bool result = await _contract.DeleteRole(roleModel.RoleName);
                if (result)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Role Deleted" };
                    return StatusCode(StatusCodes.Status200OK, response);

                }
             
                return StatusCode(StatusCodes.Status200OK, response);

            }
            catch (Exception ex)
            {

                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet]
        [Route("getroles")]
        [Authorize(Policy = "ViewRole")]
        public async Task<IActionResult> GetRoles()
        {
            ResponseApi<List<IdentityRole>> response = new ResponseApi<List<IdentityRole>>() { Status = false, Message = "Could not load" };
            try
            {
                List<IdentityRole> list = _contract.GetRoles();
                response = new ResponseApi<List<IdentityRole>>() { Status = true, Message = "loaded data", Data = list };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<IdentityRole>>() { Status = false, Message = ex.Message };

                return StatusCode(StatusCodes.Status500InternalServerError, response);

            }
        }

        [Route("assignrole")]
        [HttpPost]
        [Authorize(Policy = "AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel roleModel)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Could not assign role" };
            try
            {
                bool result = await _contract.AssignRoleToUser(roleModel.userEmail, roleModel.roleName);
                if (result)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Role Assigned" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<bool>() { Status = false, Message = ex.InnerException.ToString() };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("removerolefromuser")]
        [HttpPost]
        [Authorize(Policy = "RevokeRole")]
        public async Task<IActionResult> RemoveRoleFromUser(AssignRoleModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Could not revoke role" };
            try
            {
                UserEntity user = await _userManager.FindByEmailAsync(model.userEmail);
                if (user == null)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "User doesnot exist" };
                    return StatusCode(StatusCodes.Status200OK,response);
                }
                bool check = await _roleManager.RoleExistsAsync(model.roleName);
                if (!check)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "Role doesnot exist." };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                bool check1 = await _contract.RemoveRoleFromUser(user,model.roleName);
                if (check1)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Role succesfully revoked." };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<bool>() { Status = false, Message = ex.InnerException.ToString() };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
