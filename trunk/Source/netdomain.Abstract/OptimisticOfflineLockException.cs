//-------------------------------------------------------------------------------
// <copyright file="OptimisticOfflineLockException.cs" company="bbv Software Services AG">
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
    /// The exception that is thrown if an optimistic offline lock violation occurs.
    /// Wrappes the concurrency exception of the underlying O/R mapping framework.
    /// </summary>
    [Serializable]
    public class OptimisticOfflineLockException : Exception
    {
        /// <summary>
        /// Represents the conflicted objects.
        /// </summary>
        private readonly ConflictedObject[] confictedObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimisticOfflineLockException"/> class.
        /// </summary>
        /// <param name="confictedObjects">The conficted objects.</param>
        public OptimisticOfflineLockException(ConflictedObject[] confictedObjects)
        {
            this.confictedObjects = confictedObjects;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimisticOfflineLockException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="confictedObjects">The conficted objects.</param>
        public OptimisticOfflineLockException(string message, ConflictedObject[] confictedObjects)
            : base(message)
        {
            this.confictedObjects = confictedObjects;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimisticOfflineLockException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <param name="confictedObjects">The conficted objects.</param>
        public OptimisticOfflineLockException(string message, Exception innerException, ConflictedObject[] confictedObjects)
            : base(message, innerException)
        {
            this.confictedObjects = confictedObjects;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimisticOfflineLockException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        public OptimisticOfflineLockException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.confictedObjects = (ConflictedObject[])info.GetValue("validationResults", typeof(ConflictedObject[]));
        }

        /// <summary>
        /// Gets the conflicted objects.
        /// </summary>
        public ConflictedObject[] ConfictedObjects
        {
            get { return this.confictedObjects; }
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
            info.AddValue("validationResults", this.ConfictedObjects);
        }
    }

    /// <summary>
    /// Defines a conflicted object.
    /// </summary>
    [Serializable]
    public class ConflictedObject
    {
        /// <summary>
        /// The type of the conflicted object.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// The conflicted object.
        /// </summary>
        private readonly object conflictedObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictedObject"/> class.
        /// </summary>
        /// <param name="conflictedObject">The conflicted object.</param>
        /// <param name="type">The type of the conflicted object.</param>
        public ConflictedObject(object conflictedObject, Type type)
        {
            this.conflictedObject = conflictedObject;
            this.type = type;
        }

        /// <summary>
        /// Gets the type of the conflicted object.
        /// </summary>
        public Type Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the conflicted object.
        /// </summary>
        public object Object
        {
            get { return this.conflictedObject; }
        }
    }
}