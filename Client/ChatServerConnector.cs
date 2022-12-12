using Client.ChatServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public class ChatServerConnector : IChatServiceCallback
    {
        private static ChatServerConnector Instance = null;
        private ChatServiceClient Client;
        private int UserId;
        private string Name;
        private ChatServerConnector()
        {
            Client = new ChatServiceClient(new System.ServiceModel.InstanceContext(this));
            
        }
        static ChatServerConnector()
        {
            if (Instance == null)
                Instance = new ChatServerConnector();
        }
        public static ChatServerConnector GetInstance()
        {
            if (Instance == null)
                Instance = new ChatServerConnector();
            return Instance;
        }

        public static void Disconnect()
        {
            GetInstance().Client.Disconnect(Instance.UserId);
        }
        internal static string Connect(string text)
        {
            if (text.Length < 4)
            {
               return ErrorMessageUserName();
            }
            Instance.UserId = Instance.Client.Connect(text);
            Instance.Name = text;
            GetInstance().Client.UpdateUserList();
            return "OK";
        }
        public void SendMessageToServer(string message, int id)
        {
            GetInstance().Client.SendMessage(message, id, 1, Instance.Name);
        }

        public int SelectedUser()
        {
            return (Application.OpenForms["Form1"] as Form1).listBoxUsers.SelectedIndex;
        }
        public void SendMessageToClient(string message)
        {
            (Application.OpenForms["Form1"] as Form1).listBoxChat.Items.Add(message);
        }

        public void UpdateUserListToClient(string[] users)
        {
            (Application.OpenForms["Form1"] as Form1).listBoxUsers.Items.Clear();
            (Application.OpenForms["Form1"] as Form1).listBoxUsers.Items.Add("GroupChat---");
            (Application.OpenForms["Form1"] as Form1).listBoxUsers.SelectedIndex = 0;
            foreach (var user in users)
            {
                (Application.OpenForms["Form1"] as Form1).listBoxUsers.Items.Add(user);
            }
        }

        private static string ErrorMessageUserName()
        {
            return "UserName can't be less than 4";
        }

    }
}
    
