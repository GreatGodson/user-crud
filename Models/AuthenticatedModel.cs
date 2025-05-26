namespace MyFirstApi.Models
{
    public class AuthenticatedModel
    {
        public required User User { get; set; }
        public required string AccessToken { get; set; }

    }
}