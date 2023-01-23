using AutoMapper;
using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Services.Contracts;
using IdentityCore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityCore.Controllers
{
    [Route("api")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountContract accountContract;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IRoleContract _roleContract;
        private readonly IUserContract _userContract;

        public AccountController(
            IAccountContract _accountContract,
            IMapper mapper, 
            IConfiguration config,
            UserManager<UserEntity> userManager,
            IRoleContract contract,
            IUserContract userContract)
        {
            accountContract = _accountContract;
            _mapper = mapper;
            _config = config;
            _userManager = userManager;
            _roleContract = contract;
            _userContract = userContract;
        }
        [Route("[controller]/register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Registration failed" };
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, response);
                }
                RegisterEntity register = _mapper.Map<RegisterEntity>(registerModel);
                IdentityResult result = await accountContract.Register(register);

                if (result.Succeeded)
                {
                    bool result1 = await _roleContract.AssignRoleToUser(registerModel.Email, "User");
                    if (!result1)
                    {
                        response = new ResponseApi<bool>() { Status = true, Message = "Registered but could not assign roles" };
                        return StatusCode(StatusCodes.Status200OK, response);

                    }
                    UserEntity user = await _userManager.FindByEmailAsync(registerModel.Email);
                    bool result2 = await _userContract.AddClaimToUser(user, "hasAccessToViewUserList");
                    if (!result2)
                    {
                        response = new ResponseApi<bool>() { Status = true, Message = "could not add claims" };
                        return StatusCode(StatusCodes.Status200OK, response);
                    }
                    response = new ResponseApi<bool>() { Status = true, Message = "Registered. Roles and Claims assigned." };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                var errors = result.Errors.Select(e => e.Description);
                response = new ResponseApi<bool>() { Status = false, Message = "Validation Errors", Errors = errors };
                return StatusCode(StatusCodes.Status400BadRequest, response);




            }
            catch (Exception ex)
            {
                
                response = new ResponseApi<bool>() { Status = false, Message = ex.InnerException.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
        //update


        //login 
        [Route("[controller]/login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            ResponseApi<UserModel> response = new ResponseApi<UserModel>() { Status = false, Message = "Incorrect Email or Password",Data=null };
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,response);
                }

                bool result = await accountContract.Login(loginModel);

                if (!result)
                {
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                UserEntity user = await _userManager.FindByEmailAsync(loginModel.Email);
                UserModel resultmapped = _mapper.Map<UserModel>(user);

                IList<string> roles = await _userManager.GetRolesAsync(user);
                resultmapped.Claim = await _userManager.GetClaimsAsync(user);
                string tokenAsString = await CreateToken(user,roles,resultmapped.Claim);
                response = new ResponseApi<UserModel>() { Status = true, Message = "Login sucessful",Token=tokenAsString,Role=roles,Data=resultmapped };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<UserModel>() { Status = false, Message = ex.InnerException.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
        //internal function
        private async Task<string> CreateToken(UserEntity user,IList<string> roles,IList<Claim> claims)
        {
           // var claim = new IList<Claim>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.Add(new Claim(ClaimTypes.Name,user.Id));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)

                );
            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenAsString;
        }
       
    }
}
