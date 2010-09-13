//-------------------------------------------------------------------------------
// <copyright file="ValidationException.cs" company="bbv Software Services AG">
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

namespace netdomain.Abstract
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    /// <summary>
    /// Represents errors that occur during validation.
    /// </summary>
    /// <typeparam name="TValidationResults">The type of the validation results.</typeparam>
    [Serializable]
    public class ValidationException<TValidationResults> : Exception
    {
        /// <summary>
        /// The validation results.
        /// </summary>
        private readonly TValidationResults validationResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException&lt;TValidationResults&gt;"/> class.
        /// </summary>
        /// <param name="validationResults">The validation results.</param>
        public ValidationException(TValidationResults validationResults)
        {
            this.validationResults = validationResults;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException&lt;TValidationResults&gt;"/> class.
        /// </summary>
        /// <param name="validationResults">The validation results.</param>
        /// <param name="reason">The reason.</param>
        public ValidationException(TValidationResults validationResults, string reason) : base(reason)
        {
            this.validationResults = validationResults;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException&lt;TValidationResults&gt;"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        public ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.validationResults = (TValidationResults)info.GetValue("validationResults", typeof(TValidationResults));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException&lt;TValidationResults&gt;"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the validation results.
        /// </summary>
        public TValidationResults ValidationResults
        {
            get { return this.validationResults; }
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        [SecurityCritical, SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("validationResults", this.ValidationResults);
        }
    }
}