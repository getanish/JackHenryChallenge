namespace JackHenryChallenge.Models
{
    /// <summary>
    /// AppSettings for the application
    /// </summary>
    public class AppSettings
    {
        public TwitterConfig TwitterConfig { get; set; } = null!;

        public int NotificationDurationInSeconds { get; set; }
    }
}
