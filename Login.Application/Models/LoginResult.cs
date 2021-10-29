namespace Login.Application.Models
{
    public class LoginResult
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
