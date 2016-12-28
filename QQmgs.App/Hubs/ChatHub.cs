using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Twitter.App.BusinessLogic;

namespace Twitter.App.Hubs
{
    public class ChatHub : Hub
    {
        private static int _userNumber = 0;

        private static readonly ConcurrentDictionary<string, ChatUser> Users = new ConcurrentDictionary<string, ChatUser>(StringComparer.OrdinalIgnoreCase);

        private static int ChattingHistoryRecord { get; } = 15;

        private static int _chattingHistoryIndex = 0;

        private static readonly ConcurrentDictionary<int, ChatMessage> ChattingHistory = new ConcurrentDictionary<int, ChatMessage>();

        public static string DefaultChattingRoom { get; } = "defaultRoom";

        public bool Join()
        {
            // Loading chatting history
            Clients.Caller.loadHistory(GetChattingHistory());

            Cookie userIdCookie;

            if (!Context.RequestCookies.TryGetValue("QQmgs-chat-userid", out userIdCookie))
            {
                userIdCookie = new Cookie("QQmgs-chat-userid", "");
            }

            var user = Users.Values.FirstOrDefault(u => u.Id == userIdCookie.Value);
            if (user == null)
            {
                var nickName = GenerateRondomNickName();
                AddUser(nickName);

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

            AddChattingHistory(ChattingHistory, new ChatMessage(name, message));
        }
        
        public override Task OnConnected()
        {
            ++_userNumber;

            return null;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = Users.Values.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);

            if (user != null)
            {
                var msg = $"\"{user.Name}\"退出了群聊, 当前{_userNumber--} 人在线.";
                Clients.All.addNewMessageToPage("system", msg);

                AddChattingHistory(ChattingHistory, new ChatMessage("system", msg));
            }

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

        public int GetUserNumber()
        {
            return Users.Count;
        }

        private IEnumerable<ChatMessage> GetChattingHistory()
        {
            return ChattingHistory.Select(pair => new ChatMessage(pair.Value.User, pair.Value.Text));
        }

        private static void AddChattingHistory(IDictionary<int, ChatMessage> dictionary, ChatMessage msg)
        {
            dictionary[_chattingHistoryIndex] = msg;
            _chattingHistoryIndex++;

            // Only keep 6 latest messages in runtime memory
            if (_chattingHistoryIndex >= ChattingHistoryRecord)
            {
                dictionary.Remove(_chattingHistoryIndex - ChattingHistoryRecord);
            }
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
            Clients.Caller.registeredTime = user.RegisteredTime;

            // Add this user to the list of users
            Clients.Caller.addUser(user);
        }

        private ChatUser AddUser(string newUserName)
        {
            var user = new ChatUser(newUserName, GetMD5Hash(newUserName))
            {
                ConnectionId = Context.ConnectionId,
                RegisteredTime = DateTime.Now
            };

            Users[newUserName] = user;

            AddUserToClient(user);

            return user;
        }

        private static string GenerateRondomNickName()
        {
            var names = new List<string>
            {
                "钱江湾",
                "金沙港",
                "金字塔",
                "字母楼",
                "保研路",
                "酱饼妹",
                "裸奔男",
                "墨湖",
                "碧湖",
                "教工路",
                "学正街",
                "二号大街",
                "中门",
                "球门",
                "鸟门",
                "信息楼",
                "经济楼",
                "管理楼",
                "食品楼",
                "环境楼",
                "二号田径场"
            };

            return names.PickRandom();
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

        public DateTime RegisteredTime { get; set; }

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