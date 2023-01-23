using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityCore.DbLayer.Entity
{
    public class TempMessageStoreEntity
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }

        [ForeignKey("SenderId")]
        public UserEntity Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public UserEntity Receiver { get; set; }
    }
}
