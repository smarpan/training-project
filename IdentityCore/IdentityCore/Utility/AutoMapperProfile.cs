using AutoMapper;
using IdentityCore.DbLayer.Entity;
using IdentityCore.Migrations;
using IdentityCore.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IdentityCore.Utility
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region UserModel
            CreateMap<UserEntity, UserModel>()
                .ForMember(destiny => destiny.Email, opt => opt.MapFrom(origin => origin.Email))
                .ForMember(destiny => destiny.UserName, opt => opt.MapFrom(origin => origin.UserName))
                .ForMember(destiny => destiny.PhoneNumber, opt => opt.MapFrom(origin => origin.PhoneNumber))
                .ForMember(destiny => destiny.ProfileImage, opt => opt.MapFrom(origin => origin.ProfileImage != null && origin.ProfileImage != null ? Convert.ToBase64String(origin.ProfileImage) : Default.defaultImage))
                .ForMember(destiny => destiny.Gender, opt => opt.MapFrom(origin => origin.Gender));

            CreateMap<UserModel, UserEntity>()
                .ForMember(destiny => destiny.Email, opt => opt.MapFrom(origin => origin.Email))
                .ForMember(destiny => destiny.UserName, opt => opt.MapFrom(origin => origin.UserName))
                .ForMember(destiny => destiny.PhoneNumber, opt => opt.MapFrom(origin => origin.PhoneNumber))
                .ForMember(destiny => destiny.ProfileImage, opt => opt.MapFrom(origin => origin.ProfileImage != null && origin.ProfileImage != null ? Convert.FromBase64String(origin.ProfileImage) : Convert.FromBase64String(Default.defaultImage)))
                .ForMember(destiny => destiny.Gender, opt => opt.MapFrom(origin => origin.Gender));
            #endregion
            #region UserMini
            CreateMap<UserEntity, MiniUserModel>()
                .ForMember(destiny => destiny.Email, opt => opt.MapFrom(origin => origin.Email))
                .ForMember(destiny => destiny.ProfileImage, opt => opt.MapFrom(origin => origin.ProfileImage != null && origin.ProfileImage != null ? Convert.ToBase64String(origin.ProfileImage) : Default.defaultImage))
                .ForMember(destiny => destiny.Id, opt => opt.MapFrom(origin => origin.Id))
                .ForMember(destiny => destiny.UserName, opt => opt.MapFrom(origin => origin.UserName));
            #endregion

            #region RegistrationModel
            CreateMap<RegisterModel, RegisterEntity>()
                .ForMember(destiny => destiny.Email, opt => opt.MapFrom(origin => origin.Email))
                .ForMember(destiny => destiny.Password, opt => opt.MapFrom(origin => origin.Password))
                .ForMember(destiny => destiny.UserName, opt => opt.MapFrom(origin => origin.UserName))
                .ForMember(destiny => destiny.ConfirmPassword, opt => opt.MapFrom(origin => origin.ConfirmPassword));
            #endregion

            #region RegisterToUser
            CreateMap<RegisterEntity, UserEntity>()
                .ForMember(destiny => destiny.Email, opt => opt.MapFrom(origin => origin.Email))
                .ForMember(destiny => destiny.UserName, opt => opt.MapFrom(origin => origin.UserName))
                .ForMember(destiny => destiny.Gender, opt => opt.MapFrom(origin => origin.Gender));

            #endregion

            #region LoginModel
            CreateMap<LoginModel, UserEntity>()
                .ForMember(destiny => destiny.Email, opt => opt.MapFrom(origin => origin.Email));
            #endregion

            #region Post
            CreateMap<PostEntity, PostModel>()
                .ForMember(destiny => destiny.Image, opt => opt.MapFrom(origin => Convert.ToBase64String(origin.Image)))
                .ForMember(destiny => destiny.UserName, opt => opt.MapFrom(origin => origin.User.UserName))
                .ForMember(destiny => destiny.userProfileImage, opt => opt.MapFrom(origin => origin.User != null && origin.User.ProfileImage != null ? Convert.ToBase64String(origin.User.ProfileImage) : Default.defaultImage))
                .ForMember(destiny => destiny.Id, opt => opt.MapFrom(origin => origin.Id));
            CreateMap<PostModel, PostEntity>()
                .ForMember(destiny => destiny.Image, opt => opt.MapFrom(origin => Convert.FromBase64String(origin.Image)))
                .ForMember(destiny => destiny.UserId, opt => opt.MapFrom(origin => origin.UserId))
                .ForMember(destiny => destiny.Id, opt => opt.Ignore());
            #endregion

            #region LikePost
            CreateMap<LikePostEntity, LikeModel>();
            CreateMap<LikeModel, LikePostEntity>()
                .ForMember(destiny => destiny.Id, opt => opt.Ignore());
            CreateMap<PostActionModel, LikeModel>()
                .ForMember(destiny => destiny.PostId, opt => opt.MapFrom(origin => origin.Id))
                .ForMember(destiny => destiny.UserId, opt => opt.MapFrom(origin => origin.UserId));
            CreateMap<PostActionModel, LikePostEntity>()
                .ForMember(destiny => destiny.PostId, opt => opt.MapFrom(origin => origin.Id))
                .ForMember(destiny => destiny.UserId, opt => opt.MapFrom(origin => origin.UserId));

            #endregion

            #region ChatContact
            CreateMap<ContactEntity, ChatContactModel>()
                .ForMember(destiny => destiny.ContactUserName, opt => opt.MapFrom(origin => origin.ContactUser.UserName))
                .ForMember(destiny => destiny.ContactProfileImage, opt => opt.MapFrom(origin => origin.ContactUser != null && origin.ContactUser.ProfileImage != null ? Convert.ToBase64String(origin.ContactUser.ProfileImage) : Default.defaultImage));
            #endregion
            #region MessageStore
            CreateMap<DbLayer.Entity.MessageStoreEntity, MessageStoreModel>()
                .ForMember(destiny => destiny.ReceiverUsername, opt => opt.MapFrom(origin => origin.Receiver.UserName))
                .ForMember(destiny => destiny.SenderUsername, opt => opt.MapFrom(origin => origin.Sender.UserName));
            CreateMap<MessageStoreModel, DbLayer.Entity.MessageStoreEntity>();
            #endregion
        }
    }
}
