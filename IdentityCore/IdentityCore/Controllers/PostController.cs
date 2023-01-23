using AutoMapper;
using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Services.Contracts;
using IdentityCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostContract _postContract;
        private readonly IMapper _mapper;
        private readonly UserManager<UserEntity> _userManager;

        public PostController(IPostContract postContract,IMapper mapper, UserManager<UserEntity> userManager)
        {
            this._postContract = postContract;
            this._mapper = mapper;
            this._userManager = userManager;
        }
        [Route("addpost")]
        [HttpPost]
        [Authorize(Policy = "AddPost")]
        public async Task<IActionResult> addPost(PostModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "Could not add post" };
            try
            {
                PostEntity post = _mapper.Map<PostEntity>(model);
                bool check = await this._postContract.AddPost(post);
                if (check)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Post added" };
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

        [Route("deletemypost")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Deletepost(PostActionModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "could not delete post" };
            try
            {
                bool check = await _postContract.DeletePost(model);
                if (check)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Post deleted!" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<bool>() { Status=false,Message=ex.Message};
                return StatusCode(StatusCodes.Status500InternalServerError,response);
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPosts()
        {
            ResponseApi<List<PostModel>> response = new ResponseApi<List<PostModel>>() {Status=false,Message="Could not get posts" };
            try
            {
                List<PostModel> postEntities = await _postContract.GetPosts();
               // List<PostModel> postModelList = _mapper.Map<List<PostModel>>(postEntities);
                response = new ResponseApi<List<PostModel>>() { Status = true, Message = "request sucessful",Data= postEntities };
                return StatusCode(StatusCodes.Status200OK,response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<List<PostModel>>() { Status = true, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("profile/{id}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ProfilePosts(string id)
        {
            ResponseApi<List<PostModel>> response = new ResponseApi<List<PostModel>>() { Status = false, Message = "Could not get profile" };
            try
            {
                UserEntity check = await _userManager.FindByIdAsync(id);
                if (check == null)
                {
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                List<PostModel> postEntities = await _postContract.MyPosts(id);
                
                response = new ResponseApi<List<PostModel>>() { Status = true, Message = "request successful", Data = postEntities };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {

                response = new ResponseApi<List<PostModel>>() { Status = true, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Route("likepost")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LikePost(PostActionModel model)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = false, Message = "couldnot complete request" };
            try
            {
                bool check = await _postContract.LikePost(model);
                if (check)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Request sucessful" };
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

    }
}
