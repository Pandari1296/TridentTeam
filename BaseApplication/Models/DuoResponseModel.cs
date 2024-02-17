namespace BaseApplication.Models
{
    public class DuoResponseModel
    {
        public string AuthResponse { get; set; }
    }

    public class AuthContext
    {
        public string event_type { get; set; }
        public string reason { get; set; }
        public string result { get; set; }
    }

    public class DuoAuthResponse
    {
        public AuthContext AuthContext { get; set; }
    }
}
