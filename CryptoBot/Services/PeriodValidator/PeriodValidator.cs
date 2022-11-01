using CryptoBot.Settings;
using Microsoft.Extensions.Options;

namespace CryptoBot.Services.PeriodValidator
{
    public class PeriodValidator : IPeriodValidator
    {
        public  IOptions<TokenSendingLimit> _options;
        public PeriodValidator(IOptions<TokenSendingLimit> options)
        {
            _options = options;
        }

        public bool TryValidate(string value, out int validatedValue)
        {
            validatedValue = 0;

            var isPeriod = int.TryParse(value, out int period);
            if (!isPeriod)
            {
                return false;
            }

            var isValidPeriod = period > _options.Value.Min && period <= _options.Value.Max;
            if (!isValidPeriod)
            {
                return false;
            }

            validatedValue = period;
            return true;
        }
    }
}
