using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityCore.DbLayer.Entity
{
    public class UserEntity:IdentityUser
    {
        [Required(ErrorMessage ="Gender is required.")]
        public string Gender { get; set; }
        public byte[]? ProfileImage { get; set; }

    }
}
