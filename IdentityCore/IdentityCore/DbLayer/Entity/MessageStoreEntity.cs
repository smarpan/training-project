using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityCore.DbLayer.Entity
{
    public class MessageStoreEntity
    {
        public Guid Id { get; set; }
        public string BelongsTo { get; set; }
        public string SenderId { get; set; }
        public string  ReceiverId { get; set; }
        public string Message { get; set; }

        [ForeignKey("BelongsTo")]
        public UserEntity BelongsToUser { get; set; }

        [ForeignKey("SenderId")]
        public UserEntity Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public UserEntity Receiver { get; set; }
    }
}
