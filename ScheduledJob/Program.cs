using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using Twitter.Data.UnitOfWork;

namespace ScheduledJob
{
    public class Program
    {
        static void Main(string[] args)
        {
            JobManager.AddJob(() =>
            {
                //Do something...

                string content;

                try
                {
                    var data = new TwitterData();
                    var userNumber = data.Users.All().Count();
                    var tweetNumber = data.Tweets.All().Count();
                    //content = $"[{DateTime.Now.ToString("O")}] <br/>主人好睡觉了... 我也很困了<br/><br/>现在我们有{userNumber}位用户哦~<br/>不仅如此我们现在一共有{tweetNumber}条发言在我们的全球某工商平台哦~";

                    content = $"[{DateTime.Now.ToString("O")}] <br/><br/> 主人天天好心情，今天是 {DateTime.Today.DayOfWeek}, 可以起床了哦~";

                    MailJob.SendMailAsync(content).Wait();
                }
                catch (Exception e)
                {
                    content = e.Message;
                }

                //var sw = new StreamWriter(@"C:\Users\Administrator\Desktop\test\qqmgs.txt", true);
                //sw.WriteLine(content);
                //sw.Close();

                Console.WriteLine(content, DateTime.Now);

            }, t =>
            {
                //从程序启动开始执行，然后每秒钟执行一次
                //t.ToRunNow().AndEvery(1).Minutes();
                var time = new DateTime(2016, 7, 26, 6, 30, 0);
                t.ToRunOnceAt(time).AndEvery(1).Days();
                ////带有任务名称的任务定时器
                //t.WithName("TimerTask").ToRunOnceAt(DateTime.Now.AddSeconds(5));
            });
            Console.ReadKey();
        }
    }
}
