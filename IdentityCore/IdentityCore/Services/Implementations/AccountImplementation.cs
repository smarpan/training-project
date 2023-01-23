using AutoMapper;
using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityCore.Services.Implementations
{
    public class AccountImplementation : IAccountContract
    {
		private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;
        public AccountImplementation(UserManager<UserEntity> userManager,IMapper mapper)
		{
			_userManager= userManager;
			_mapper= mapper;
        }

		public async Task<List<UserEntity>> GetUserList()
		{
			try
			{
				List<UserEntity> users =await _userManager.Users.ToListAsync();
				return users;
			}
			catch (Exception ex)
			{

				throw ex;
			}
		}

        public async Task<bool> Login(LoginModel loginModel)
		{
			try
			{
				
				UserEntity user = await _userManager.FindByEmailAsync(loginModel.Email);
				if (user == null)
				{
					return false;
				}
				bool result = await _userManager.CheckPasswordAsync(user, loginModel.Password);
				if (result)
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
		
        public async Task<IdentityResult> Register(RegisterEntity register)
        {
			try
			{
				UserEntity user = _mapper.Map<UserEntity>(register);
				//var user = new UserEntity { UserName=register.Email,Email=register.Email };
				return await _userManager.CreateAsync(user,register.Password);
				 
			}
			catch (Exception ex)
			{
				
				throw ex;
			}
        }

       
    }
}
