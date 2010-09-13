//-------------------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="bbv Software Services AG">
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

namespace netdomain.Helpers
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The class ReflectionHelper implements useful functionality to use reflection 
    /// e.g. to get a private member.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets a non public instance property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="holder">The holder of the property to get the value from.</param>
        /// <returns>The value of the property as an object.</returns>
        public static object GetNoPublicInstanceProperty(string propertyName, object holder)
        {
            return holder.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(holder, null);
        }

        /// <summary>
        /// Gets a non public instance property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="holder">The holder of the property.</param>
        /// <param name="args">Optional arguments for indexed properties.</param>
        /// <returns>The value of the property as an object.</returns>
        public static object GetNoPublicInstanceProperty(string propertyName, object holder, object[] args)
        {
            return holder.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(holder, args);
        }

        /// <summary>
        /// Gets a non public instance field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="holder">The holder of the field to get the value from.</param>
        /// <returns>The value of the field as an object.</returns>
        public static object GetNoPublicInstanceField(string fieldName, object holder)
        {
            return holder.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(holder);
        }

        /// <summary>
        /// Calls a non public instance method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="obj">The holder of the method to call.</param>
        /// <param name="arguments">The arguments to pass into the method.</param>
        /// <returns>The return value of the method as an object.</returns>
        public static object CallNoPublicInstanceMethod(string methodName, object obj, params object[] arguments)
        {
            return obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, arguments);
        }

        /// <summary>
        /// Calls a non public instance method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="obj">The holder of the method to call.</param>
        /// <param name="type">The generic type.</param>
        /// <param name="arguments">The arguments to pass into the method.</param>
        /// <returns>The return value of the method as an object.</returns>
        public static object CallNoPublicGenericInstanceMethod(string methodName, object obj, Type type, params object[] arguments)
        {
            return obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type).Invoke(obj, arguments);
        }

        /// <summary>
        /// Calls a public instance method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="obj">The holder of the method to call.</param>
        /// <param name="arguments">The arguments to pass into the method.</param>
        /// <returns>The return value of the method as an object.</returns>
        public static object CallPublicInstanceMethod(string methodName, object obj, params object[] arguments)
        {
            return obj.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, obj, arguments);
        }

        /// <summary>
        /// Sets the no public instance field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="holder">The holder of the field.</param>
        /// <param name="value">The value to set the field to.</param>
        public static void SetNoPublicInstanceField(string fieldName, object holder, object value)
        {
            holder.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(holder, value);
        }

        /// <summary>
        /// Creates a delegate of a mehod using reflection.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="obj">The instance to access the method.</param>
        /// <returns>A delegate of type T</returns>
        public static T CreateDelegateOfPublicStaticMethod<T>(string methodName, object obj) where T : class
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("The type argument must be derived from System.Delegate");
            }

            var methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            return Delegate.CreateDelegate(typeof(T), methodInfo, false) as T;
        }

        /// <summary>
        /// Creates a delegate of a mehod using reflection.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="obj">The instance to access the method.</param>
        /// <returns>A delegate of type T</returns>
        public static T CreateDelegateOfNonPublicInstanceMethod<T>(string methodName, object obj) where T : class
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("The type argument must be derived from System.Delegate");
            }

            var methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            return Delegate.CreateDelegate(typeof(T), obj, methodInfo, false) as T;
        }

        /// <summary>
        /// Creates a setter delegate of non public field of the base class of a type.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="holder">The holder of the field.</param>
        /// <returns>A delegate of type <see cref="T:System.Action"/> </returns>
        public static Action<object, object> CreateSetterDelegateOfNonPublicBaseTypeField(string fieldName, object holder)
        {
            var fieldInfo = holder.GetType().BaseType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            var methodInfo = fieldInfo.GetType().GetMethod("SetValue", new[] { typeof(object), typeof(object) });
            
            return (Action<object, object>)Delegate.CreateDelegate(typeof(Action<object, object>), fieldInfo, methodInfo, false);
        }
    }
}
