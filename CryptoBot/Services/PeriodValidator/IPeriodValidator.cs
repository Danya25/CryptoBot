namespace CryptoBot.Services.PeriodValidator
{
    public interface IPeriodValidator
    {
        bool TryValidate(string value, out int validatedValue);
    }
}
