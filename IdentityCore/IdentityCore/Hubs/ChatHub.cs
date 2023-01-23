using IdentityCore.Interface;
using IdentityCore.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace IdentityCore.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private static List<string> online_users = new List<string>();
        private readonly IUserContract _userContract;

        public ChatHub(IUserContract userContract)
        {           
             this._userContract = userContract;
        }


        public override async Task<Task> OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;
            if (!online_users.Contains(name))
            {
                online_users.Add(name);
            }
            Groups.AddToGroupAsync(Context.ConnectionId, name);
           // await fetchPendingMessages();
            return base.OnConnectedAsync();
        }
        //public async Task fetchPendingMessages()
        //{

        //    List<DbLayer.Entity.TempMessageStoreEntity> messages = await _userContract.GetTempMessages(Context.User.Identity.Name);
        //    foreach (DbLayer.Entity.TempMessageStoreEntity message in messages)
        //    {
        //        SendMessageAsync(message.RecieverId, message.SenderId, message.SenderUsername, message.Message);
        //    }
        //}

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            online_users.Remove(Context.User.Identity.Name);
            Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            return base.OnDisconnectedAsync(exception);
        }

        public void Greetings()
        {

            Clients.Caller.DisplayGreeting("You are now connected with connectionId:- " + Context.ConnectionId);
        }

        public async Task SendMessageAsync(string receiverId, string senderId, string senderUsername, string message)
        {
            bool check = await _userContract.CheckIfUserExists(receiverId);
            bool check1 = await _userContract.CheckIfUserExists(senderId);
            bool check3 = online_users.Contains(receiverId);
            if (check && check1 && !check3)
            {
                await _userContract.AddTempMessage(receiverId, senderId, senderUsername, message);
            }
            if (check3)
            {
                await Clients.Group(receiverId).RecievedMessage(senderId, senderUsername, message);
            }


        }

    }
}
