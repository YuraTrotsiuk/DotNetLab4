using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatService
    {
        [OperationContract]
        int Connect(string username);
        [OperationContract(IsOneWay = true)]
        void Disconnect(int id);
        [OperationContract(IsOneWay = true)]
        void SendMessage(string message, int id, int systBool, string name);

        [OperationContract(IsOneWay = true)]
        void UpdateUserList();
    }

    [ServiceContract]
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendMessageToClient(string message);

        [OperationContract(IsOneWay = true)]
        void UpdateUserListToClient(List<string> users);

    }

    public class ChatUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public OperationContext Context { get; set; }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {

        List<ChatUser> usersList = new List<ChatUser>();
        int nextUserId = 1;
        public int Connect(string username)
        {
            ChatUser user = new ChatUser()
            {
                Id = nextUserId++,
                Name = username,
                Context = OperationContext.Current,
            };
            SendMessage($"---{user.Name}--- joined to the chat!", 0, 0);
            usersList.Add(user);
            return user.Id;
        }

        public void Disconnect(int id)
        {
            var user = usersList.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                usersList.Remove(user);
                SendMessage($"---{user.Name}--- Disconnected", 0, 0);
            }
        }

        public void SendMessage(string message, int id, int Sysbool, string nameSender = "")
        {
            string time = System.DateTime.Now.ToShortTimeString();
            string answer;

            if (id == 0)
            {
                foreach (ChatUser user in usersList)
                {
                    if (Sysbool == 0)
                    {
                        answer = message;
                        user.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient(answer);

                    }
                    else
                    {
                        answer = $"{time}| {nameSender} => {message}";
                        user.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient(answer);

                    }
                }
            }
            else
            {
                var userFrom = usersList.FirstOrDefault(x => x.Name == nameSender);
                var userTo = usersList.FirstOrDefault(x => x.Id == id);
                answer = $"{time}| Personal messages from {userFrom.Name} to {userTo.Name}:\n" +
                    $"{message}";
                userTo.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient(answer);
                userFrom.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient(answer);
            }

        }


        public void UpdateUserList()
        {
            List<string> list = new List<string>();
            foreach(var user in usersList)
            {
                list.Add(user.Name);
            }
            foreach (ChatUser user in usersList)
            {

                user.Context.GetCallbackChannel<IChatServiceCallback>().UpdateUserListToClient(list);
            }
        }
    }
}