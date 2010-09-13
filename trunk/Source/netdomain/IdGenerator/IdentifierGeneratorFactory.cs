//-------------------------------------------------------------------------------
// <copyright file="IdentifierGeneratorFactory.cs" company="bbv Software Services AG">
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
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.IdGenerator
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using log4net;

    /// <summary>
    /// Factory methods for <c>IdentifierGenerator</c> framework.
    /// </summary>
    public sealed class IdentifierGeneratorFactory : IIdentifierGeneratorFactory
    {
        /// <summary>
        /// Instantiate the logger.
        /// </summary>
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Provide a lock object to implement thread saved access to the singleton instance.
        /// </summary>
        private static readonly object padLock = new object();

        /// <summary>
        /// Holds a reference to the singleton of this class.
        /// </summary>
        private static IIdentifierGeneratorFactory instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierGeneratorFactory"/> class.
        /// </summary>
        internal IdentifierGeneratorFactory()
        {
            // cannot be instantiated from the outside.
        }

        /// <summary>
        /// Gets or sets the instance of the <see cref="GlobalContext"/>.
        /// </summary>
        /// <value>The instance of the <see cref="GlobalContext"/>.</value>
        public static IIdentifierGeneratorFactory Instance
        {
            get
            {
                lock (padLock)
                {
                    return instance ?? (instance = new IdentifierGeneratorFactory());
                }
            }

            set
            {
                lock (padLock)
                {
                    instance = value;
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="IIdentifierGenerator"/> from a particular type.
        /// </summary>
        /// <param name="generatorType">Type of the generator.</param>
        /// <returns>
        /// An instantiated and configured <see cref="IIdentifierGenerator"/>.
        /// </returns>
        /// <exception cref="IdentifierGenerationException">
        /// Thrown if there are any exceptions while creating the <see cref="IIdentifierGenerator"/>.
        /// </exception>
        public IIdentifierGenerator Create(Type generatorType)
        {
            try
            {
                return (IIdentifierGenerator)Activator.CreateInstance(generatorType);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(CultureInfo.InvariantCulture, "Could not instantiate id generator of type {0}", generatorType.DeclaringType);
                throw new IdentifierGenerationException("Could not instantiate id generator", e);
            }
        }
    }
}