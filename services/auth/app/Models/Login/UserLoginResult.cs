namespace app.Models.Login
{
    public class UserLoginResult
    {
        public bool IsAuthenticated { get; set; }
        public string? JwtToken { get; set; }
        public string? ErrorMessage { get; set; }
    }
}