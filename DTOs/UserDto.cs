namespace API.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; } // main photo to display in nav-bar
        public string KnownAs { get; set; }
    }
}