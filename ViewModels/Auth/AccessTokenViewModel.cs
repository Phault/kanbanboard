namespace Kanbanboard.ViewModels.Auth
{
    public class AccessTokenViewModel
    {
        public string AccessToken { get; set; }
        public long ExpiresAt { get; set; }
    }
}