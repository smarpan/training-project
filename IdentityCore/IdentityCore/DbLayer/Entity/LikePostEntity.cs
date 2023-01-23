using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityCore.DbLayer.Entity
{
    public class LikePostEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }

        [ForeignKey("PostId")]
        public ICollection<PostEntity> Posts { get; set; }
        
        [ForeignKey("UserId")]
        public ICollection<UserEntity> Users { get; set; }
    }
}
