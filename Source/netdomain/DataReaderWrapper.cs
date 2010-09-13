//-------------------------------------------------------------------------------
// <copyright file="DataReaderWrapper.cs" company="bbv Software Services AG">
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

    /// <summary>
    /// Implements a class to wrapp an enumerator into a IDataReader interface.
    /// </summary>
    public class DataReaderWrapper : IDataReader
    {
        /// <summary>
        /// The enumerator.
        /// </summary>
        private readonly IEnumerator enumerator;

        /// <summary>
        /// The propertyinfos.
        /// </summary>
        private readonly PropertyInfo[] propertyInfos;

        /// <summary>
        /// The delegate cache of all getter methods.
        /// </summary>
        private readonly List<Delegate> delegateCache = new List<Delegate>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderWrapper"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public DataReaderWrapper(IEnumerable collection)
        {
            this.enumerator = collection.GetEnumerator();
            var type = collection.GetType().GetGenericArguments()[0];
            this.propertyInfos = type.GetProperties();

            this.CreateDelegateCache(type);
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.
        /// </returns>
        public int FieldCount
        {
            get { return this.propertyInfos.Length; }
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The level of nesting.
        /// </returns>
        public int Depth
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <value></value>
        /// <returns>true if the data reader is closed; otherwise, false.
        /// </returns>
        public bool IsClosed
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.
        /// </returns>
        public int RecordsAffected
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified i.
        /// </summary>
        /// <value></value>
        object IDataRecord.this[int i]
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        object IDataRecord.this[string name]
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public string GetName(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public string GetDataTypeName(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public Type GetFieldType(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Object"/> which will contain the field value upon return.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public object GetValue(int i)
        {
            return this.delegateCache[i].DynamicInvoke(this.enumerator.Current);
        }

        /// <summary>
        /// Gets all the attribute fields in the collection for the current record.
        /// </summary>
        /// <param name="values">An array of <see cref="T:System.Object"/> to copy the attribute fields into.</param>
        /// <returns>
        /// The number of instances of <see cref="T:System.Object"/> in the array.
        /// </returns>
        public int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The index of the named field.</returns>
        public int GetOrdinal(string name)
        {
            return Array.FindIndex(this.propertyInfos, new Predicate<PropertyInfo>(fi => fi.Name == name));
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public bool GetBoolean(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public byte GetByte(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public char GetChar(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of characters read.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The GUID value of the specified field.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public short GetInt16(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public int GetInt32(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public long GetInt64(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public float GetFloat(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public double GetDouble(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The string value of the specified field.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public string GetString(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public decimal GetDecimal(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public DateTime GetDateTime(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an <see cref="T:System.Data.IDataReader"/> for the specified column ordinal.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/>.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>.
        /// </exception>
        public bool IsDBNull(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Closes the <see cref="T:System.Data.IDataReader"/> Object.
        /// </summary>
        public void Close()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable"/> that describes the column metadata of the <see cref="T:System.Data.IDataReader"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable"/> that describes the column metadata.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The <see cref="T:System.Data.IDataReader"/> is closed.
        /// </exception>
        public DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public bool NextResult()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public bool Read()
        {
            return this.enumerator.MoveNext();
        }

        /// <summary>
        /// Creates a cache of all getter methods as delegates.
        /// </summary>
        /// <param name="type">The generic type of the enumerable.</param>
        private void CreateDelegateCache(Type type)
        {
            var param = Expression.Parameter(type, "c");

            foreach (var propertyInfo in this.propertyInfos)
            {
                Expression member = Expression.Property(param, propertyInfo.Name);
                this.delegateCache.Add(Expression.Lambda(member, param).Compile());
            }
        }
    }
}