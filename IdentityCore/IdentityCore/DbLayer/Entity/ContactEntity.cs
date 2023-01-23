using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityCore.DbLayer.Entity
{
    public class ContactEntity
    {
        public Guid Id { get;set; }
        
        public string UserId { get; set; }

        public string ContactUserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity ContactBelongToUser { get; set; }
        
        [ForeignKey("ContactUserId")]        
        public UserEntity ContactUser { get; set; }
    }
}
