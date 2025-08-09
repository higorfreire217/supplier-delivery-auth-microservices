namespace app.Models.Login
{
    public class UserLoginResult
    {
        public bool IsAuthenticated { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}