namespace IdentityCore.Interface
{
    public interface IChatHub
    {
        Task DisplayGreeting(string userName);
        Task RecievedMessage(string senderId, string senderUsername, string message);
    }
}
