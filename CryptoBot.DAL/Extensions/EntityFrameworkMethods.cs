using Microsoft.EntityFrameworkCore;

namespace CryptoBot.DAL.Extensions
{
    public static class EntityFrameworkMethods
    {
        public static Task<TOriginal> GetFirstOrDefaultASync<TOriginal>(this IQueryable<TOriginal> source)
        {
            return EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(source);
        }
        public static void AddOrUpdate<T>(this ApplicationContext ctx, T entity) where T : class
        {
            var entry = ctx.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case EntityState.Added:
                    ctx.Add(entity);
                    break;
                case EntityState.Unchanged:
                    //item already in db no need to do anything  
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
