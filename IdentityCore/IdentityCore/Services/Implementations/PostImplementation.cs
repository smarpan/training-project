using AutoMapper;
using IdentityCore.DbLayer.DbServices;
using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IdentityCore.Services.Implementations
{
    public class PostImplementation : IPostContract
    {
        private readonly MainContext _context;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;

        public PostImplementation(MainContext context, UserManager<UserEntity> userManager, IMapper mapper)
        {
            this._context = context;
            this._userManager = userManager;
            this._mapper = mapper;
        }

        public async Task<bool> AddPost(PostEntity entity)
        {
            try
            {
                _context.posts.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<PostModel>> GetPosts()
        {
            try
            {
                List<PostEntity> posts = new List<PostEntity>();
                posts = _context.posts.Include(data => data.User).OrderByDescending(x => x.Id).ToList();
                List<PostModel> postModelList = _mapper.Map<List<PostModel>>(posts);
                foreach (var post in postModelList)
                {
                    post.likes = await _context.likePostsAndUsers.Where(x => x.PostId == post.Id).Select(x => x.UserId).ToListAsync();

                }
                return postModelList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> DeletePost(PostActionModel model)
        {
            try
            {
                PostEntity check = _context.posts.Where(e => e.Id == model.Id).FirstOrDefault();
                if (check == null)
                {
                    return false;

                }
                if (check.UserId == model.UserId)
                {
                    _context.posts.Remove(check);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<PostModel>> MyPosts(string id)
        {
            try
            {
                List<PostEntity> posts = _context.posts.Where(p => p.UserId == id).OrderByDescending(x => x.Id).ToList();
                List<PostModel> postModelList = _mapper.Map<List<PostModel>>(posts);
                foreach (var post in postModelList)
                {
                    post.likes = await _context.likePostsAndUsers.Where(x => x.PostId == post.Id).Select(x => x.UserId).ToListAsync();

                }
                return postModelList;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> LikePost(PostActionModel model)
        {
            try
            {
                LikeModel likeModel = _mapper.Map<LikeModel>(model);
                var check = _context.likePostsAndUsers.Where(x => x.PostId == likeModel.PostId && x.UserId == likeModel.UserId).FirstOrDefault();
                if (check == null)
                {

                    LikePostEntity entry = _mapper.Map<LikePostEntity>(likeModel);
                    _context.likePostsAndUsers.Add(entry);
                    await _context.SaveChangesAsync();
                    return true;
                }
                _context.Remove(check);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
