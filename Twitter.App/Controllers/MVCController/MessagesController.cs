namespace Twitter.App.Controllers
{
    using Twitter.Data.UnitOfWork;

    public class MessagesController : TwitterBaseController
    {
        public MessagesController()
            : base(new TwitterData())
        {
        }
    }
}