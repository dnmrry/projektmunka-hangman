using hangman.Models;
using Microsoft.EntityFrameworkCore;

namespace hangman.Data
{
    public class AppDbContext : DbContext
    {
        public static string? ConnectionString { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConnectionString);
        }
    }


}