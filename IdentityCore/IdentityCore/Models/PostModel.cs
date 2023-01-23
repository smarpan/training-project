using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IdentityCore.Models
{
    public class PostModel
    {
        public PostModel()
        {
            DateCreated = DateTime.Now;
        }
        public string? Id { get; set; }
        public string? Status { get; set; }
        public string? Image { get; set; }
        public string? userProfileImage { get; set; }
        public string? UserName { get; set; }
        public string UserId { get; set; }

        public List<string>? likes { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
