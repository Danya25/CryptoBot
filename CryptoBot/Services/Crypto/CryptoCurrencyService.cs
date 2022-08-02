using CoinGecko.Clients;
using CoinGecko.Entities.Response.Simple;
using CryptoBot.Models;
using CryptoBot.Utils;

namespace CryptoBot.Crypto.Services
{
    public class CryptoCurrencyService : ICryptoCurrencyService
    {
        private HttpClient _httpClient;
        public CryptoCurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<CryptoToken>> GetTokenPrice(string[] tokensId, string[] currencies)
        {
            var pingClient = new PingClient(_httpClient, JsonSerializer.GetSerializerSettings());
            var simpleClient = new SimpleClient(_httpClient, JsonSerializer.GetSerializerSettings());

            if ((await pingClient.GetPingAsync()).GeckoSays == string.Empty)
                return new List<CryptoToken>(0);

            var simplePrice = await simpleClient.GetSimplePrice(tokensId, currencies);
            var parsedPrice = ParseCryptoToken(simplePrice);

            return parsedPrice;
        }

        private List<CryptoToken> ParseCryptoToken(Price? price)
        {
            var prices = new List<CryptoToken>();

            if (price == null)
                return prices;

            foreach (var token in price)
            {
                var values = token.Value;
                var CryptoToken = new CryptoToken
                {
                    UsdPrice = values["usd"].Value,
                    Name = token.Key.ToUpper(),
                };
                prices.Add(CryptoToken);
            }

            return prices;
        }
    }
}
