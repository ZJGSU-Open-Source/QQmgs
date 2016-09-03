namespace Twitter.App.Controllers
{
    using Twitter.Data.UnitOfWork;

    public class TwitterBaseController : BaseController
    {
        protected TwitterBaseController(IQQmgsData data)
        {
            this.Data = data;
        }

        protected IQQmgsData Data { get; }
    }
}