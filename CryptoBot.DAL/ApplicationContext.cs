using CryptoBot.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoBot.DAL
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> opts) : base(opts)
        {
            //Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPostInfo> UserPostsInfo { get; set; }
        public DbSet<Token> Tokens { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            new UserConfiguration().Configure(builder.Entity<User>());
            builder.ApplyConfiguration(new TokenConfiguration());
        }

    }

    public class ApplicationDbContextFactory : IDbContextFactory<ApplicationContext>
    {
        private DbContextOptions<ApplicationContext> _options;
        public ApplicationDbContextFactory(DbContextOptions<ApplicationContext> opts)
        {
            _options = opts;
        }

        public ApplicationContext CreateDbContext()
        {
            return new ApplicationContext(_options);
        }
    }
}
