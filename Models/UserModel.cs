using System.Text.Json.Serialization;

namespace MyFirstApi.Models

{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Role { get; set; }
        public required string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; } = string.Empty;



    }
}