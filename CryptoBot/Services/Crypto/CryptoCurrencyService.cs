using CoinGecko.Clients;
using CoinGecko.Entities.Response.Simple;
using CoinGecko.Interfaces;
using CryptoBot.Models;
using CryptoBot.Utils;

namespace CryptoBot.Crypto.Services
{
    public class CryptoCurrencyService : ICryptoCurrencyService
    {
        private ISimpleClient _simpleClient;
        private IPingClient _pingClient;
        private ICoinsClient _coinsClient;
        public CryptoCurrencyService(ISimpleClient simpleClient, IPingClient pingClient, ICoinsClient coinsClient)
        {
            _simpleClient = simpleClient;
            _pingClient = pingClient;
            _coinsClient = coinsClient;
        }
        public async Task<List<CryptoToken>> GetTokenPrice(string[] tokensId, string[] currencies)
        {
            if ((await _pingClient.GetPingAsync()).GeckoSays == string.Empty)
                return new List<CryptoToken>(0);

            var simplePrice = await _simpleClient.GetSimplePrice(tokensId, currencies);
            var parsedPrice = ParseCryptoToken(simplePrice);

            return parsedPrice;
        }

        public async Task<string> GetTokenPrice(string id)
        {
            if ((await _pingClient.GetPingAsync()).GeckoSays == string.Empty)
                return null;


            var simplePrice = await _coinsClient.GetAllCoinDataWithId("bitcoin");
            var usdPrice = simplePrice.MarketData.CurrentPrice["usd"];
            var tt = simplePrice;

            return "";
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
