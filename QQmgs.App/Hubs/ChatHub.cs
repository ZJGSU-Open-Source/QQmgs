using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Twitter.App.Hubs
{
    public class ChatHub : Hub
    {
        // Current online user number
        private static int _userNumber = 0;

        private static readonly ConcurrentDictionary<string, ChatUser> Users = new ConcurrentDictionary<string, ChatUser>(StringComparer.OrdinalIgnoreCase);

        public static string DefaultChattingRoom { get; } = "defaultRoom";

        public bool Join()
        {
            Cookie userIdCookie;

            if (!Context.RequestCookies.TryGetValue("QQmgs-chat-userid", out userIdCookie))
            {
                userIdCookie = new Cookie("QQmgs-chat-userid", "");
            }

            var user = Users.Values.FirstOrDefault(u => u.Id == userIdCookie.Value);
            if (user == null)
            {
                AddUser(GetMD5Hash(DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                return false;
            }

            // Update the users's client id mapping if already connected
            user.ConnectionId = Context.ConnectionId;
            
            AddUserToClient(user);

            return true;
        }

        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
        
        public override Task OnConnected()
        {
            ++_userNumber;

            return null;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.addNewMessageToPage("【系统消息】", $"有一位小伙伴离开了群聊, {_userNumber--} 人当前在线.");

            var user = Users.Values.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);

            //if (user != null)
            //{
            //    ChatUser ignoredUser;
            //    Users.TryRemove(user.Name, out ignoredUser);
            //}

            return null;
        }

        public IEnumerable<ChatUser> GetUsers()
        {
            var users = Users.Select(user => new ChatUser
            {
                Id = user.Value.Id,
                Name = user.Value.Name,
                ConnectionId = user.Value.ConnectionId,
                Hash = user.Value.Hash
            });

            return users;
        }

        private static string GetMD5Hash(string name)
        {
            return string.Join("", MD5.Create()
                .ComputeHash(Encoding.Default.GetBytes(name))
                .Select(b => b.ToString("x2")));
        }

        private void AddUserToClient(ChatUser user)
        {
            Clients.Caller.id = user.Id;
            Clients.Caller.name = user.Name;
            Clients.Caller.hash = user.Hash;

            // Add this user to the list of users
            Clients.Caller.addUser(user);
        }

        private ChatUser AddUser(string newUserName)
        {
            var user = new ChatUser(newUserName, GetMD5Hash(newUserName))
            {
                ConnectionId = Context.ConnectionId
            };

            Users[newUserName] = user;

            AddUserToClient(user);

            return user;
        }
    }

    [Serializable]
    public class ChatMessage
    {
        public string Id { get; private set; }

        public string User { get; set; }

        public string Text { get; set; }

        public ChatMessage(string user, string text)
        {
            User = user;
            Text = text;
            Id = Guid.NewGuid().ToString("d");
        }
    }

    [Serializable]
    public class ChatUser
    {
        public string ConnectionId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Hash { get; set; }

        public ChatUser()
        {
        }

        public ChatUser(string name, string hash)
        {
            Name = name;
            Hash = hash;
            Id = Guid.NewGuid().ToString("d");
        }
    }
}