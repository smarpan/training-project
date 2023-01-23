namespace IdentityCore.Models
{
    public class MessageStoreModel
    {
        public Guid? Id { get; set; }
        public string? BelongsTo { get; set; }
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverUsername { get; set; }
        public string Message { get; set; }
    }
}
