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
        public string? Currency { get; set; }
        public string CryptoSet { get; set; }
        public DateTime LastPostTime { get; set; }

        [NotMapped]
        public List<string> CryptoSetCollection
        {
            get
            {
                return CryptoSet.Split(";").ToList();
            }
        }

        public bool RemoveCryptoAsset(string value)
        {
            var cryptoAssets = CryptoSet.Split(";").ToList();
            var isDeleted = cryptoAssets.Remove(value);
            if(!isDeleted)
                return false;


            CryptoSet = string.Join(";", cryptoAssets);
            return true;
        }
       
        public bool AddCryptoAsset(string value)
        {
            if (CryptoSetCollection.Contains(value))
                return false;

            CryptoSet = $"{CryptoSet};{value}";
            return true;
        }

    }
}
