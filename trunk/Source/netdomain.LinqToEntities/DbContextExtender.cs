namespace netdomain.LinqToEntities
{
    using System;
    using System.Collections;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Helper class to extend missing functionality of the <see cref="DbContextExtender"/>.
    /// </summary>
    public static class DbContextExtender
    {
        /// <summary>
        /// Create a delegate to clear the cache of the ObjectContext.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>An delegate of type <see cref="T:System.Action"/></returns>
        public static Action CreateClearCacheDelegate(this DbContext context)
        {
             var objectContext = ((IObjectContextAdapter)context).ObjectContext;

             var sets =
                ((object)context).GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
                    pi => pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).Select(pi => pi.GetValue(context, null));

            return () =>
                {
                    foreach (IEnumerable set in sets)
                    {
                        foreach (var entity in set)
                        {
                            objectContext.Detach(entity);
                        }
                    }
                };
        }
    }
}