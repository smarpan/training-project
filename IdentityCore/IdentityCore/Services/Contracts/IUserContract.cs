using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Utility;
using System.Security.Claims;

namespace IdentityCore.Services.Contracts
{
    public interface IUserContract
    {
        Task<bool> AddClaimToUser(UserEntity user,string claimName);
        Task<bool> AddContact(string userId, string contactId);
        Task<bool> AddTempMessage(string recieverId, string senderId, string senderUsername, string message);
        Task<ResponseApi<bool>> AddToMessageStore(MessageStoreModel user);
        Task<bool> CheckIfUserExists(string id);
        Task<bool> deleteContact(string userId, string contactId);
        Task<ResponseApi<bool>> DeleteFromMessageStore(string userId, string contactId);
        Task<IList<Claim>> GetClaimForUser(UserEntity user);
        Task<ContactEntity> GetContact(string userId, string contactId);
        Task<List<ContactEntity>> GetContacts(string userId);
        Task<ResponseApi<List<MessageStoreModel>>> GetFromMessageStore(string userId);
        Task<List<TempMessageStoreEntity>> GetTempMessages(string recieverId);
        Task<bool> RemoveClaimFromUser(UserEntity user, string claimName);
        Task<bool> UpdateUserInfo(UserEntity user);
    }
}
