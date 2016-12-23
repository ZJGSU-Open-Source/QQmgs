using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Twitter.App.Hubs
{
    public class ChatHub : Hub
    {
        private static int _userNumber = 0;

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

            return null;
        }
    }
}