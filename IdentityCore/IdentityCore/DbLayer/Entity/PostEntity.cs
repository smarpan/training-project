using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityCore.DbLayer.Entity
{
    public class PostEntity
    {      
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string? Status { get; set; }
        public byte[]? Image { get; set; }
        public DateTime DateCreated { get; set; }        

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }


    }
}
