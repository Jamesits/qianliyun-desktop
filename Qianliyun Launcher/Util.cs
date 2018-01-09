using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Pathoschild.Http.Client;

namespace Qianliyun_Launcher
{
    static class ExtensionMethod
    {
        public static Type GetEnumeratedType<T>(this IEnumerable<T> _)
        {
            return typeof(T);
        }
    }

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

        public static bool StringInArray(string value, string[] from, bool ignoreCase = false)
        {
            if (ignoreCase)
            {
                from = from.Select(x => x.ToLower()).ToArray();
                value = value.ToLower();
            }
            return Array.IndexOf(from, value) > -1;
        }


        // get only the property you need in a JSON string
        public static string RipJsonObject(string json, string propertyName)
        {
            return JObject.Parse(json).GetValue(propertyName).ToString();
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
#if DEBUG
            Logger.Debug("HTTP {0} {1} \nRequest Header: {2}\nResponse header: {3}", 
                response.Message.Version,
                response.Message.StatusCode,
                response.Message.RequestMessage,
                response.Message.Headers);
#endif
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
#if DEBUG
            Logger.Debug("Trying to access member {0} type {1}", binder.Name, binder.GetType());
#endif
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
#if DEBUG
            Logger.Debug("Trying to invoke member {0} type {1}", binder.Name, binder.GetType());
#endif
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
        where T : class, new()
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

        private static Type GetEnumeratedType<U>(IEnumerable<U> _)
        {
            return typeof(U);
        }

        public static void MapList(ExpandoObject source, List<T> destination)
        {
            var src_enum = source as IEnumerable<T>;
            if (src_enum == null)
            {
                Logger.Fatal("source is not a list");
                throw new ArgumentException("source is not a list");
            }
            foreach (var item in src_enum)
            {
                var ret0 = new T();
                //Map(item, ret0);
                destination.Add(ret0);
            }
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
#if DEBUG
                        Logger.Warn("Setting an not nullable type property `{0}` to null, ignored", kv.Key);
#endif
                        // Throw if type is a value type 
                        // but not Nullable<>
                        // throw new ArgumentException($"`{kv.Key}` is not nullable");
                    }
                }
                else if (kv.Value.GetType() != propType)
                {
#if DEBUG
                    Logger.Warn("Assigning property `{0}` with type `{1}` to a different type `{2}`", kv.Key, kv.Value.GetType(), propType);
#endif
                    // You could make this a bit less strict 
                    // but I don't recommend it.
                    // throw new ArgumentException("type mismatch");
                }
                p.SetValue(destination, kv.Value, null);
            }
        }
    }
}
