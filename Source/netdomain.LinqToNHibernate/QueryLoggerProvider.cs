namespace netdomain.LinqToNHibernate
{
    using NHibernate;
    using NHibernate.SqlCommand;

    public class QueryLoggerProvider : EmptyInterceptor
    {
        private readonly QueryLogger queryLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryLoggerProvider"/> class.
        /// </summary>
        /// <param name="queryLogger">The query logger.</param>
        public QueryLoggerProvider(QueryLogger queryLogger)
        {
            this.queryLogger = queryLogger;
        }

        /// <summary>
        /// Called when [prepare statement].
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            var sqlStatement = sql.ToString().ToCharArray();
            this.queryLogger.Write(sqlStatement, 0, sqlStatement.Length);
            return sql;
        }
    }

}