using AutoMapper;
using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Services.Contracts;
using IdentityCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace IdentityCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin,User")]
    public class UserController : ControllerBase
    {
        private readonly IAccountContract accountContract;
        private readonly IMapper _mapper;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IUserContract _userContract;

        public UserController(IAccountContract _accountContract, IUserContract userContract, IMapper mapper, UserManager<UserEntity> userManager)
        {
            accountContract = _accountContract;
            _mapper = mapper;
            _userManager = userManager;
            _userContract = userContract;
        }

        [Route("getusers")]
        [HttpGet]
        [Authorize(Policy = "ViewUserList")]
        public async Task<IActionResult> GetUsers()
        {
            ResponseApi<List<MiniUserModel>> response = new ResponseApi<List<MiniUserModel>>() { Status = false, Message = "Request failed" };
            try
            {
                //var userId = User.Claims;
                List<UserEntity> users = await accountContract.GetUserList();
                List<MiniUserModel> userModel = _mapper.Map<List<MiniUserModel>>(users);
                if (users.Count > 0)
                {
                    //for(var user=0;user<users.Count;user++){
                    //    userModel[user].Role = await _userManager.GetRolesAsync(users[user]);
                    //    userModel[user].Claim = await _userManager.GetClaimsAsync(users[user]);
                    //}

                    response = new ResponseApi<List<MiniUserModel>>() { Status = true, Message = "Request succesful", Data = userModel };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<List<MiniUserModel>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("getuserminibyid/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetUserMiniById(string id)
        {
            ResponseApi<MiniUserModel> response = new ResponseApi<MiniUserModel>() { Status = false, Message = "Request failed" };
            try
            {
                //var userId = User.Claims;
                UserEntity user = await _userManager.FindByIdAsync(id.Trim());
                if (user == null)
                {
                    response = new ResponseApi<MiniUserModel>() { Status = false, Message = "User not found with id" + id };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                MiniUserModel usermapped = _mapper.Map<MiniUserModel>(user);
                response = new ResponseApi<MiniUserModel>() { Status = true, Message = "User Found", Data = usermapped };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<MiniUserModel>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("getuserbyemail/{email}")]
        [HttpGet]
        public async Task<IActionResult> GetUser(string email)
        {
            ResponseApi<UserModel> response = new ResponseApi<UserModel>() { Status = true, Message = "Request failed" };

            try
            {
                UserEntity user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    response = new ResponseApi<UserModel>() { Status = false, Message = "User not found by email" + email };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                UserModel usermapped = _mapper.Map<UserModel>(user);
                usermapped.Role = await _userManager.GetRolesAsync(user);
                usermapped.Claim = await _userManager.GetClaimsAsync(user);
                response = new ResponseApi<UserModel>() { Status = true, Message = "User Found", Data = usermapped };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<UserModel>() { Status = false, Message = "Request failed" };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("getuserbyid/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            ResponseApi<UserModel> response = new ResponseApi<UserModel>() { Status = false, Message = "Request failed" };
            try
            {
                UserEntity user = await _userManager.FindByIdAsync(id.Trim());
                if (user == null)
                {
                    response = new ResponseApi<UserModel>() { Status = false, Message = "User not found with id" + id };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                UserModel usermapped = _mapper.Map<UserModel>(user);
                usermapped.Role = await _userManager.GetRolesAsync(user);
                usermapped.Claim = await _userManager.GetClaimsAsync(user);
                response = new ResponseApi<UserModel>() { Status = true, Message = "User Found", Data = usermapped };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<UserModel>() { Status = false, Message = "Request failed" };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [Route("checkavailableemail/{email}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AvailableEmail(string email)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = email + " is not available" };
            try
            {
                UserEntity result = await _userManager.FindByEmailAsync(email);
                if (result == null)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = email + " available" };
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


        [Route("addclaimtouser")]
        [HttpPost]
        [Authorize(Policy = "AddClaimToUser")]
        public async Task<IActionResult> AddClaimToUser(ClaimUserModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Claim already exist." };
            try
            {
                UserEntity user = await _userManager.FindByEmailAsync(model.UserEmail);
                if (user == null)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "user doesnot exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                bool check = await _userContract.AddClaimToUser(user, model.ClaimName);
                if (check)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "claim added" };
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


        [Route("removeclaimfromuser")]
        [HttpPost]
        [Authorize(Policy = "RemoveClaimFromUser")]
        public async Task<IActionResult> RemoveClaimFromUser(ClaimUserModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Claim does not exist for user." };
            try
            {
                UserEntity user = await _userManager.FindByEmailAsync(model.UserEmail);
                if (user == null)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "user does not exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                bool check = await _userContract.RemoveClaimFromUser(user, model.ClaimName);
                if (check)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "claim deleted" };
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

        [Route("updateuserinfo")]
        [HttpPut]
        [Authorize(Policy = "UpdateUserData")]
        public async Task<IActionResult> UpdateUserInfo(UserModel user)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "could not update user" };
            try
            {
                UserEntity checkIfUserExist = await _userManager.FindByEmailAsync(user.Email);
                if (checkIfUserExist == null)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "user doesnot exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                checkIfUserExist.UserName = user.UserName.Trim();
                checkIfUserExist.PhoneNumber = user.PhoneNumber.Trim();
                checkIfUserExist.Gender = user.Gender.Trim();
                checkIfUserExist.ProfileImage = Convert.FromBase64String(user.ProfileImage);
                //UserEntity userEntity = _mapper.Map<UserEntity>(checkIfUserExist);
                bool check = await _userContract.UpdateUserInfo(checkIfUserExist);
                if (check)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "User Updated Sucessfully", };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status200OK, response);
            }
        }

        [Route("addcontact/{userId}/{contactId}")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddContact(string userId, string contactId)
        {
            ResponseApi<ChatContactModel> response = new ResponseApi<ChatContactModel>() { Status = false, Message = "Contact already exist." };
            try
            {
                var ifContactExist = await _userManager.FindByIdAsync(contactId);
                if (ifContactExist == null)
                {
                    response = new ResponseApi<ChatContactModel>() { Status = false, Message = "Contact does not Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                var ifUserExist = await _userManager.FindByIdAsync(userId);
                if (ifUserExist == null)
                {
                    response = new ResponseApi<ChatContactModel>() { Status = false, Message = "User does not Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                var check = await _userContract.AddContact(userId, contactId);
                if (check)
                {
                    ContactEntity contact = await _userContract.GetContact(userId, contactId);
                    ChatContactModel model = _mapper.Map<ChatContactModel>(contact);
                    response = new ResponseApi<ChatContactModel>() { Status = true, Message = "Contact added", Data = model };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<ChatContactModel>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("deletecontact/{userId}/{contactId}")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteContact(string userId, string contactId)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Contact does not exist in your messenger." };
            try
            {
                var ifContactExist = await _userManager.FindByIdAsync(contactId);
                if (ifContactExist == null)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "Contact does not Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                var ifUserExist = await _userManager.FindByIdAsync(userId);
                if (ifUserExist == null)
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "User does not Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                var check = await _userContract.deleteContact(userId, contactId);
                if (check)
                {

                    response = new ResponseApi<bool>() { Status = true, Message = "Contact deleted" };
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

        [Route("getcontacts/{userId}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetContacts(string userId)
        {
            ResponseApi<List<ChatContactModel>> response = new ResponseApi<List<ChatContactModel>>() { Status = false, Message = "Could not get contacts" };
            try
            {
                var ifUserExist = await _userManager.FindByIdAsync(userId);
                if (ifUserExist == null)
                {
                    response = new ResponseApi<List<ChatContactModel>>() { Status = false, Message = "User does not Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                var result = await _userContract.GetContacts(userId);
                List<ChatContactModel> chatContacts = _mapper.Map<List<ChatContactModel>>(result);
                response = new ResponseApi<List<ChatContactModel>>() { Status = true, Message = "contacts on the way", Data = chatContacts };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<List<ChatContactModel>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("getpendingmessages/{userId}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPendingMessages(string userId)
        {
            ResponseApi<List<DbLayer.Entity.TempMessageStoreEntity>> response = new ResponseApi<List<DbLayer.Entity.TempMessageStoreEntity>>() { Status = false, Message = "Could not get messages" };
            try
            {
                List<DbLayer.Entity.TempMessageStoreEntity> messages = await _userContract.GetTempMessages(userId);
                response = new ResponseApi<List<DbLayer.Entity.TempMessageStoreEntity>>() { Status = true, Message = "Messages arrived", Data = messages };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<List<DbLayer.Entity.TempMessageStoreEntity>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("messagestore/{userId}")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMessagesToMessageStore(string userId, [FromBody] MessageStoreModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() {Status=false,Message="request failed" };
            try
            {
                model.BelongsTo = userId;
                response = await _userContract.AddToMessageStore(model);
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<bool>() {Status=false,Message=ex.InnerException.ToString() };
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            }
        }

        [Route("getmessages/{userId}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMessagesFromMessageStore(string userId)
        {
            ResponseApi<List<MessageStoreModel>> response = new ResponseApi<List<MessageStoreModel>>() { Status = false, Message = "request failed" };
            try
            {
                response = await _userContract.GetFromMessageStore(userId);
                return StatusCode(StatusCodes.Status200OK,response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<List<MessageStoreModel>>() { Status = false, Message = ex.InnerException.ToString() };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }

}


