//-------------------------------------------------------------------------------
// <copyright file="DbConnectionExtension.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 Roger Rudin, bbv Software Services AG
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;
    using Abstract;

    /// <summary>
    /// Extends the IConnectionManager.
    /// </summary>
    public static class DbConnectionExtension
    {
        /// <summary>
        /// Caches the delegate.
        /// </summary>
        private static readonly IDictionary<Type, Delegate> delegateCache = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Executes the stored procedure, and returns the first column of the first row in the
        /// resultset returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="text">The command text e.g. the name of the store procedure or sql query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The first column of the first row in the resultset.</returns>
        public static TResult ExecuteScalar<TResult>(this IConnectionManager manager, string text, params Parameter[] parameters)
        {
            IDbCommand command = PrepareCommand(manager.Connection, text, parameters);
            return (TResult)ExecuteMethod<object>(command.ExecuteScalar, manager);
        }

        /// <summary>
        /// Executes a stored procedure against the Connection object of a .NET Framework
        /// data provider, and returns the number of rows affected.
        /// </summary>
        /// <remarks>This method uses a linq expression tree to generate the mapping
        /// delegate. There is a small delay while getting the first recordset
        /// because the expression tree have to be compiled to a delegate.
        /// Do use the mapping features of your O/R framwork instead!</remarks>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="manager">The connection manager.</param>
        /// <param name="text">The command text e.g. the name of the store procedure or sql query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An enumerable of a generic type.</returns>
        public static IEnumerable<TResult> Execute<TResult>(this IConnectionManager manager, string text, params Parameter[] parameters)
        {
            IDbCommand command = PrepareCommand(manager.Connection, text, parameters);

            Delegate del;

            lock (((ICollection)delegateCache).SyncRoot)
            {
                delegateCache.TryGetValue(typeof(TResult), out del);
            }

            var connected = ManageConnection(manager);
            IDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var values = new object[reader.FieldCount];

                if (del == null)
                {
                    NewExpression result = Expression.New(typeof(TResult));
                    var memberBindings = new MemberBinding[reader.FieldCount];
                    var parms = new ParameterExpression[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        //var par = Expression.Parameter(reader.GetValue(i).GetType(), "p");
                        var par = Expression.Parameter(typeof(TResult).GetProperty(reader.GetName(i)).PropertyType, "p");

                        PropertyInfo parMember =
                            typeof(TResult).GetProperty(reader.GetName(i));

                        MemberBinding parMemberBinding = Expression.Bind(
                            parMember,
                            par);

                        memberBindings[i] = parMemberBinding;
                        parms[i] = par;
                    }

                    MemberInitExpression memberInitExpression = Expression.MemberInit(
                        result,
                        memberBindings);

                    del = Expression.Lambda(memberInitExpression, parms).Compile();
                    lock (((ICollection)delegateCache).SyncRoot)
                    {
                        delegateCache.Add(typeof(TResult), del);
                    }
                }

                reader.GetValues(values);
                values.Replace(o => o.GetType() == typeof(DBNull), null);

                yield return (TResult)del.DynamicInvoke(values);
            }

            reader.Close();
            ManageConnection(manager, connected);
        }

        /// <summary>
        /// Executes a stored procedure against the Connection object of a .NET Framework
        /// data provider, and returns the number of rows affected.
        /// </summary>
        /// <param name="manager">The connection manager.</param>
        /// <param name="text">The command text e.g. the name of the store procedure or sql query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The connection does not exist.  -or- The connection is not open.
        /// </exception>
        public static int ExecuteNonQuery(this IConnectionManager manager, string text, params Parameter[] parameters)
        {
            IDbCommand command = PrepareCommand(manager.Connection, text, parameters);
            return ExecuteMethod<int>(command.ExecuteNonQuery, manager);
        }

        /// <summary>
        /// Replaces the specified objects.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="items">The items of the array.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="value">The value to replace.</param>
        public static void Replace<T>(this T[] items, Predicate<T> predicate, T value)
        {
            for (var i = 0; i < items.Length - 1; i++)
            {
                if (predicate(items[i]))
                {
                    items[i] = value;
                }
            }
        }

        /// <summary>
        /// Prepares the stored procedure command.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="text">The command text e.g. the name of the store procedure or sql query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A command object of type <see cref="T:System.Data.IDbCommand"/></returns>
        private static IDbCommand PrepareCommand(IDbConnection connection, string text, IEnumerable<Parameter> parameters)
        {
            IDbCommand command = connection.CreateCommand();
            //command.CommandType = CommandType.StoredProcedure;
            command.CommandText = text;

            foreach (var param in parameters)
            {
                IDbDataParameter parameter = command.CreateParameter();
                parameter.ParameterName = param.Name;
                parameter.Value = param.Value;
                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Executes the method.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="method">The method delegate to execute.</param>
        /// <param name="manager">The connection manager.</param>
        /// <returns>An object of the type T according to the strategy method.</returns>
        private static T ExecuteMethod<T>(Func<T> method, IConnectionManager manager)
        {
            var connected = ManageConnection(manager);
            T result = method();
            ManageConnection(manager, connected);

            return result;
        }

        /// <summary>
        /// Manages the connection.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="connected">if set to <c>true</c> [connected].</param>
        private static void ManageConnection(IConnectionManager manager, bool connected)
        {
            if (!connected)
            {
                manager.Disconnect();
            }
        }

        /// <summary>
        /// Manages the connection.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>True if the Connection was already open, otherwise false.</returns>
        private static bool ManageConnection(IConnectionManager manager)
        {
            var connected = manager.IsConnected;
            manager.Reconnect();
            return connected;
        }
    }

    /// <summary>
    /// Defines a parameter returned as a result from a stored procedure.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>The value of the parameter.</value>
        public object Value { get; set; }
    }
}
