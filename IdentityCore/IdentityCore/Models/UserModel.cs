using System.Security.Claims;

namespace IdentityCore.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }   
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string Gender { get; set; }
        public IList<string>? Role { get; set; }
        public IList<Claim>? Claim { get; set; }
    }
}
