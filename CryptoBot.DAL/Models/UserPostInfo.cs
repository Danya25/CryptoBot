using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoBot.DAL.Models
{
    public class UserPostInfo
    {
        [Key]
        public long UserId { get; set; }
        public User User { get; set; }
        public int Timer { get; set; }
        public string Currency { get; set; }
        public string CryptoSet { get; set; }
        public DateTime LastPostTime { get; set; }

        [NotMapped]
        public string[] CryptoSetArray
        {
            get
            {
                return CryptoSet.Split(";");
            }
            set
            {
                CryptoSet = String.Join(";", value);
                CryptoSetArray = value;
            }
        }
    }
}
