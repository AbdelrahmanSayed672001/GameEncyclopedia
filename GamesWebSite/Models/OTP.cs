namespace GamesWebSite.Models
{
    public class OTP
    {
        public int Id { get; set; }
        public string VerifyCode { get; set; }


        public int UserId { get; set; }
        public User User { get; set; }
    }
}
