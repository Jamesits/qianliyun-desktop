﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Pathoschild.Http.Client;

namespace Qianliyun_Launcher
{
    class Util
    {
        public static string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }

    class FluentHttpClientCustomFilter : Pathoschild.Http.Client.Extensibility.IHttpFilter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static StateManager State => StateManager.Instance;
        /// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
        /// <param name="request">The HTTP request.</param>
        public void OnRequest(IRequest request)
        {
            if (State.IsLoggedIn && State.LoginCredential.Length > 0)
            {
                request.WithHeader("Cookie", State.LoginCredential);
            }
        }

        /// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="httpErrorAsException">Whether HTTP error responses (e.g. HTTP 404) should be raised as exceptions.</param>
        public void OnResponse(IResponse response, bool httpErrorAsException)
        {
            Logger.Debug("HTTP {0} {1} \nRequest Header: {2}\nResponse header: {3}", 
                response.Message.Version,
                response.Message.StatusCode,
                response.Message.RequestMessage,
                response.Message.Headers);
            return;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class SBStructure<T> : DynamicObject where T : new()
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        // ReSharper disable once InconsistentNaming
        public string error;
        private readonly T _obj;

        // Implement the TryGetMember method of the DynamicObject class for dynamic member calls.
        public override bool TryGetMember(GetMemberBinder binder,
            out object result)
        {
            result = null;
            Logger.Debug("Trying to access member {0} type {1}", binder.Name, binder.GetType());
            if (binder.GetType() != _obj.GetType()) return false;
            result = _obj;
            return true;
        }

        // Implement the TryInvokeMember method of the DynamicObject class for 
        // dynamic member calls that have arguments.
        public override bool TryInvokeMember(InvokeMemberBinder binder,
            object[] args,
            out object result)
        {
            Logger.Debug("Trying to invoke member {0} type {1}", binder.Name, binder.GetType());
            result = null;
            return false;
        }

        public SBStructure()
        {
            _obj = new T();
        }
    }

    // By using a generic class we can take advantage
    // of the fact that .NET will create a new generic type
    // for each type T. This allows us to avoid creating
    // a dictionary of Dictionary<string, PropertyInfo>
    // for each type T. We also avoid the need for the 
    // lock statement with every call to Map.
    public static class Mapper<T>
        // We can only use reference types
        where T : class
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Dictionary<string, PropertyInfo> _propertyMap;

        static Mapper()
        {
            // At this point we can convert each
            // property name to lower case so we avoid 
            // creating a new string more than once.
            _propertyMap =
                typeof(T)
                    .GetProperties()
                    .ToDictionary(
                        p => p.Name.ToLower(),
                        p => p
                    );
        }

        public static void Map(ExpandoObject source, T destination)
        {
            // Might as well take care of null references early.
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            // By iterating the KeyValuePair<string, object> of
            // source we can avoid manually searching the keys of
            // source as we see in your original code.
            foreach (var kv in source)
            {
                PropertyInfo p;
                if (!_propertyMap.TryGetValue(kv.Key.ToLower(), out p)) continue;
                var propType = p.PropertyType;
                if (kv.Value == null)
                {
                    if (!propType.IsByRef && !(propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))) // originally used `!propType.IsByRef && propType.Name != "Nullable`1"`
                    {
                            
                        Logger.Warn("Setting an not nullable type property `{0}` to null, ignored", kv.Key);
                        // Throw if type is a value type 
                        // but not Nullable<>
                        // throw new ArgumentException($"`{kv.Key}` is not nullable");
                    }
                }
                else if (kv.Value.GetType() != propType)
                {
                    Logger.Warn("Assigning property `{0}` with type `{1}` to a different type `{2}`", kv.Key, kv.Value.GetType(), propType);
                    // You could make this a bit less strict 
                    // but I don't recommend it.
                    // throw new ArgumentException("type mismatch");
                }
                p.SetValue(destination, kv.Value, null);
            }
        }
    }
}
