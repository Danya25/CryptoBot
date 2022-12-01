using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
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
            await GetPingAsync();

            var simplePrice = await _simpleClient.GetSimplePrice(tokensId, currencies);
            var parsedPrice = ParseCryptoToken(simplePrice);

            return parsedPrice;
        }

        public async Task<CoinFullDataById> GetTokenInfo(string id)
        {
            await GetPingAsync();

            var simplePrice = await _coinsClient.GetAllCoinDataWithId(id);

            return simplePrice;
        }


        private async Task GetPingAsync()
        {
            if ((await _pingClient.GetPingAsync()).GeckoSays == string.Empty)
                return;
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
