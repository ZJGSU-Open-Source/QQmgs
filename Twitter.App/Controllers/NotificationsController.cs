namespace Twitter.App.Controllers
{
    using Twitter.Data.UnitOfWork;

    public class NotificationsController : TwitterBaseController
    {
        public NotificationsController()
            : base(new TwitterData())
        {
        }

    }
}