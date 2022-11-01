namespace CryptoBot.Utils
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T Value { get; set; }


        public Result<T> SetSuccess(T value)
        {
            Success = true;
            Value = value;
            return this;
        }

        public Result<T> SetError(T value)
        {
            Success = false;
            Value = value;
            return this;
        }

        public static Result<T> GetSuccessResult(T value)
        {
            return new Result<T>()
            {
                Success = true,
                Value = value
            };
        }

        public static Result<T> GetErrorResult(T value)
        {
            return new Result<T>()
            {
                Success = false,
                Value = value
            };
        }
    }

    public static class ResultExtensions
    {
        public static Result<T> ToSuccessMethodResult<T>(this T value)
        {
            return Result<T>.GetSuccessResult(value);
        }

        public static Result<T> ToErrorMethodResult<T>(this T value)
        {
            return Result<T>.GetErrorResult(value);
        }
    }
}
