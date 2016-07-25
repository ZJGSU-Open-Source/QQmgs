using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Twitter.App;

namespace ScheduledJob
{
    public class MailJob
    {
        private static readonly EmailService EmailService = new EmailService();

        public static async Task SendMailAsync(string content)
        {
            const string testDestination = "100103398@qq.com";
            const string subject = "Bot alert minutes";
            var body = content;

            await EmailService.SendCustomizedMailAsync(subject, body, testDestination);
        }
    }
}
