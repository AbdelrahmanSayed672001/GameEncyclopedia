using System.ComponentModel.DataAnnotations.Schema;

namespace GamesWebSite.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }


        public string? imgSrc { get; set; }

        [NotMapped]
        public IFormFile img { get; set; }



        public string? frontSrc { get; set; }

        [NotMapped]
        public IFormFile frontImg { get; set; }



        public string? backSrc { get; set; }

        [NotMapped]
        public IFormFile backImg { get; set; }

        public OTP oTP { get; set; }


    }
}
