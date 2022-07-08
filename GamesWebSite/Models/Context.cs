using Microsoft.EntityFrameworkCore;

namespace GamesWebSite.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }


        public DbSet<User> users { get; set; }
        public DbSet<OTP> oTPs { get; set; }
    }
}
