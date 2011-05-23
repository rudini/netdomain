namespace netdomain.LinqToEntities
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;

    public static class DbContextExtender
    {
        public static void ClearCache(this DbContext context)
        {
            // foreach set 

            var sets =
                context.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
                    pi => pi.PropertyType == typeof(DbSet<>)).Select(pi => pi.GetValue(context, null));

            foreach (var set in sets)
            {
                var local = set.GetType().GetProperty("Local", BindingFlags.Public | BindingFlags.Instance);
                local.GetType().GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance).Invoke(local, null);
            }
        }
    }
}