namespace PinterestDesktopBot.PinterestClient.Api.Exceptions
{
    public class PinterestNotFoundException : PinterestException
    {
        public PinterestNotFoundException(string message)
            : base(message)
        {
            HttpStatusCode = 404;
        }
    }
}
