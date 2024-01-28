namespace BaseApplication.Helpers
{
    public static class OtpGenerator
    {
        private static readonly Random random = new Random();

        public static string GenerateOtp()
        {
            // Generate a random 6-digit numeric OTP
            return random.Next(100000, 1000000).ToString();
        }
    }
}
