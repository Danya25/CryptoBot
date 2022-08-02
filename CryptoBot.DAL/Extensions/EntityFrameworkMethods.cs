using Microsoft.EntityFrameworkCore;

namespace CryptoBot.DAL.Extensions
{
    public static class EntityFrameworkMethods
    {
        public static Task<TOriginal> GetFirstOrDefaultASync<TOriginal>(this IQueryable<TOriginal> source)
        {
            return EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(source);
        }
    }
}
