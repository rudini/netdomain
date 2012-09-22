namespace netdomain.LinqToEntities
{
    using System.Data.Objects;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// 
    /// </summary>
    public class EFQueryProvider : IQueryProvider
    {
        /// <summary>
        /// The underlying query provider.
        /// </summary>
        private readonly IQueryProvider queryProvider;

        /// <summary>
        /// The query logger 
        /// </summary>
        private QueryLogger queryLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFQueryProvider"/> class.
        /// </summary>
        /// <param name="queryProvider">The query provider.</param>
        /// <param name="queryLogger">The query logger.</param>
        public EFQueryProvider(IQueryProvider queryProvider, QueryLogger queryLogger)
        {
            this.queryProvider = queryProvider;
            this.queryLogger = queryLogger;
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public IQueryable CreateQuery(Expression expression)
        {
            return this.queryProvider.CreateQuery(expression);
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var queryable = this.queryProvider.CreateQuery<TElement>(expression);

            var sqlStatement = ((ObjectQuery)queryable).ToTraceString().ToCharArray();
            this.queryLogger.Write(sqlStatement, 0, sqlStatement.Length);
            
            return queryable;
        }

        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public object Execute(Expression expression)
        {
            return this.queryProvider.Execute(expression);
        }

        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public TResult Execute<TResult>(Expression expression)
        {
            return this.queryProvider.Execute<TResult>(expression);
        }
    }
}