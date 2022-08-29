namespace JackHenryChallenge.Entities.Twitter
{
    public class TwitterResponse<T> where T : class
    {
        public T Data { get; set; } = null!;
    }
}
