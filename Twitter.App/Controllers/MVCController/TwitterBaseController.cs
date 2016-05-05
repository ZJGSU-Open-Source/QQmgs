namespace Twitter.App.Controllers
{
    using Twitter.Data.UnitOfWork;

    public class TwitterBaseController : BaseController
    {
        protected TwitterBaseController(ITwitterData data)
        {
            this.Data = data;
        }

        protected ITwitterData Data { get; }
    }
}