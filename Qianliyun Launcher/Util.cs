using System;
using System.Collections.Generic;
using System.Linq;
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
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
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
    class SBStructure<T> where T : new()
    {
        // ReSharper disable once InconsistentNaming
        public string error;
        private T _obj;
        public T this[string propertyName]
        {
            get => _obj;
            set => _obj = value;
        }
        public SBStructure()
        {
            _obj = new T();
        }
    }
}
