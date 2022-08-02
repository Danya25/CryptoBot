using CryptoBot.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBot.Validators
{
    public static class PeriodValidator
    {
        public static bool TryValidate(string value, IOptions<TokenSendingLimit> options, out int validatedValue)
        {
            validatedValue = 0;

            var isPeriod = int.TryParse(value, out int period);
            if (!isPeriod)
            {
                return false;
            }

            var isValidPeriod = period > options.Value.Min && period <= options.Value.Max;
            if (!isValidPeriod)
            {
                return false;
            }

            validatedValue = period;
            return true;
        }
    }
}
