using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;

namespace IdentityCore.Services.Contracts
{
    public interface IPostContract
    {
        Task<bool> AddPost(PostEntity entity);      
        Task<bool> DeletePost(PostActionModel model);       
        Task<List<PostModel>> GetPosts();
        Task<bool> LikePost(PostActionModel model);
        Task<List<PostModel>> MyPosts(string id);
    }
}
