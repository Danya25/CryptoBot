using CoinGecko.Entities.Response.Coins;
using CryptoBot.Models;

namespace CryptoBot.Crypto.Services
{
    public interface ICryptoCurrencyService
    {
        Task<List<CryptoToken>> GetTokenPrice(string[] tokensId, string[] currencies);
        Task<CoinFullDataById> GetTokenInfo(string id);

    }
}
