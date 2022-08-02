using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace CryptoBot.DAL.Models
{

    public class User
    {
        [Key]
        public long TelegramId { get; set; }
        public string Name { get; set; }
        public UserPostInfo PostInfo { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(t => t.TelegramId).ValueGeneratedNever();
        }
    }
}
