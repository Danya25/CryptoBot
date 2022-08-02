using CryptoBot.Models;

namespace CryptoBot.Crypto.Services
{
    public interface ICryptoCurrencyService
    {
        Task<List<CryptoToken>> GetTokenPrice(string[] tokensId, string[] currencies);
    }
}
