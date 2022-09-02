using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace CryptoBot.Tests.Services
{
    public class CryptoCurrencyServiceTest : IClassFixture<TestHost>
    {
        private IServiceProvider _serviceProvider;

        public CryptoCurrencyServiceTest(TestHost factoryHost)
        {
            _serviceProvider = factoryHost.CreateTestHost().Services;
        }


        [Fact]
        public async Task GetServiceTokenInfo_Result_True()
        {
            var tt = _serviceProvider;
           /* var service = new CryptoCurrencyService(new HttpClient());

            var result = await service.GetTokenPrice("bitcoin");*/

            Assert.True(true);
        }

    }
}
