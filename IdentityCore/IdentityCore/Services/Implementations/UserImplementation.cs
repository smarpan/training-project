using AutoMapper;
using IdentityCore.DbLayer.DbServices;
using IdentityCore.DbLayer.Entity;
using IdentityCore.Models;
using IdentityCore.Services.Contracts;
using IdentityCore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityCore.Services.Implementations
{
    public class UserImplementation : IUserContract
    {

        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;
        private readonly MainContext _context;

        public UserImplementation(UserManager<UserEntity> userManager, MainContext context,IMapper mapper)
		{
            _userManager = userManager;
            _mapper = mapper;
            this._context = context;
        }

        
        public async Task<bool> AddClaimToUser(UserEntity user, string claimName)
        {
			try
			{
                claimName = claimName.Trim();
                IList<Claim> allClaim = await _userManager.GetClaimsAsync(user);
                List<string> cName = new List<string>();
                foreach(var i in allClaim)
                {
                    cName.Add(i.Type.ToString());
                }
                
                bool check = cName.Contains(claimName);
                if (check)
                {
                    //claim already exist.
                    return false;
                }
                Claim claim = new Claim(claimName, "true");
                IdentityResult check1 = await _userManager.AddClaimAsync(user, claim);
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

        
        public async Task<bool> RemoveClaimFromUser(UserEntity user,string claimName)
        {
            try
            {
                claimName = claimName.Trim();
                IList<Claim> allClaim = await _userManager.GetClaimsAsync(user);
                List<string> cName = new List<string>();
                foreach (var i in allClaim)
                {
                    cName.Add(i.Type.ToString());
                }

                bool check = cName.Contains(claimName);
                if (!check)
                {
                    //claim does not exist.
                    return false;
                }
                Claim claim = new Claim(claimName, "true");
                IdentityResult check1 = await _userManager.RemoveClaimAsync(user, claim);
                if (check1.Succeeded)
                {
                    //claim successfully from user.
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
        public async Task<IList<Claim>> GetClaimForUser(UserEntity user)
        {
            try
            {
                IList<Claim> allClaim = await _userManager.GetClaimsAsync(user);
                return allClaim;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> UpdateUserInfo(UserEntity user)
        {
            try
            {
               
                IdentityResult check = await _userManager.UpdateAsync(user);
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

        public async Task<bool> AddContact(string userId, string contactId)
        {
            try
            {
                var ifAlreadyExist = _context.chatContactsOfUser.Where(x => x.UserId == userId && x.ContactUserId == contactId).FirstOrDefault();
                if (ifAlreadyExist!=null)
                {
                    return false;
                }
                ContactEntity entity = new ContactEntity();
                entity.UserId = userId;
                entity.ContactUserId = contactId;
                _context.chatContactsOfUser.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<ContactEntity> GetContact(string userId, string contactId)
        {
            try
            {
                var check = _context.chatContactsOfUser.Include(x=>x.ContactUser).Where(x => x.UserId == userId && x.ContactUserId == contactId).FirstOrDefault();
                return check;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<bool> deleteContact(string userId, string contactId)
        {
            try
            {
                ContactEntity ifDoesnotExist = _context.chatContactsOfUser.Where(x => x.UserId == userId && x.ContactUserId == contactId).FirstOrDefault();
                if (ifDoesnotExist == null)
                {
                    return false;
                }
                _context.chatContactsOfUser.Remove(ifDoesnotExist);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<ContactEntity>> GetContacts(string userId)
        {
            try
            {
                List<ContactEntity> contacts = await _context.chatContactsOfUser.Include(x => x.ContactUser).Where(x=>x.UserId==userId).ToListAsync();
                return contacts;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> CheckIfUserExists(string id)
        {
            try
            {
                UserEntity user = await _userManager.FindByIdAsync(id);
                if (user != null)
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

        public async Task<bool> AddTempMessage(string receiverId, string senderId, string senderUsername, string message)
        {
            try
            {
                DbLayer.Entity.TempMessageStoreEntity entity = new DbLayer.Entity.TempMessageStoreEntity();
                entity.SenderId = senderId;
                entity.ReceiverId = receiverId;
                entity.Message = message;
                entity.SenderUsername = senderUsername;
                _context.tempMessages.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<DbLayer.Entity.TempMessageStoreEntity>> GetTempMessages(string receiverId)
        {
            try
            {                
                List<DbLayer.Entity.TempMessageStoreEntity> tempMessages = await _context.tempMessages.Where(x => x.ReceiverId.Trim() == receiverId.Trim()).ToListAsync();
                _context.tempMessages.RemoveRange(tempMessages);
                await _context.SaveChangesAsync();
                return tempMessages;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<ResponseApi<bool>> AddToMessageStore(MessageStoreModel user)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() { Status = true, Message = " message added successfully" };
            try
            {
                MessageStoreEntity userEntity = _mapper.Map<MessageStoreEntity>(user);
                _context.messageStore.Add(userEntity);
                await _context.SaveChangesAsync();
                return response;
            }
            catch (Exception ex)
            {

                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseApi<List<MessageStoreModel>>> GetFromMessageStore(string userId)
        {
            ResponseApi<List<MessageStoreModel>> response = new ResponseApi<List<MessageStoreModel>>() { Status = true, Message = "messages arrived" };
            try
            {
                List<MessageStoreEntity> messages = await _context.messageStore.Include(x=>x.Sender).Include(x=>x.Receiver).Where(x => x.BelongsTo == userId).ToListAsync();
                List<MessageStoreModel> messageModel = _mapper.Map<List<MessageStoreModel>>(messages);
                response.Data = messageModel;
                return response;
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<MessageStoreModel>>() { Status = false, Message = ex.Message };
                return response;
            }
        }

        public async Task<ResponseApi<bool>> DeleteFromMessageStore(string userId,string contactId)
        {
            ResponseApi<bool> response = new ResponseApi<bool>() {Status=true,Message=" selected messages deleted successfully" };
            try
            {
                List<MessageStoreEntity> entity = _context.messageStore.Where(x => x.BelongsTo == userId &&(x.SenderId==contactId || x.ReceiverId==contactId)).ToList();
                foreach(MessageStoreEntity i in entity)
                {
                    _context.messageStore.Remove(i);
                }
                await _context.SaveChangesAsync();
                return response;
            }
            catch (Exception ex)
            {

                response = new ResponseApi<bool>() {Status=false,Message=ex.Message };
                return response;
            }
        }
    }
}
