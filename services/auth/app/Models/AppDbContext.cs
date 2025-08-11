using Microsoft.EntityFrameworkCore;
using app.Models.Register;
using app.Models.Login;

namespace app.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
    }
}