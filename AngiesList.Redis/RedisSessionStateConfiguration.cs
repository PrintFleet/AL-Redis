﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

namespace AngiesList.Redis
{
    /// <summary>
    /// Contains configuration for the session state provider.
    /// If untouched, this simply reads the sessionState section from web.config.
    /// Optionally, the <see cref="Configure"/> method can be called using
    /// PreApplicationStartMethodAttribute to programmatically configure the session
    /// state provider.
    /// </summary>
    public class RedisSessionStateConfiguration : RedisConfiguration 
    {
        #region Singleton
        private RedisSessionStateConfiguration() : base()
        { 
            CookieMode = HttpCookieMode.UseCookies;
            SessionTimeout = 60;
        }

        private static RedisSessionStateConfiguration Instance;
        #endregion

        #region Config properties
        /// <summary>
        /// ASP.NET Cookie mode
        /// </summary>
        public HttpCookieMode CookieMode { get; set;} 

        /// <summary>
        /// Session timeout (minutes). Defaults to 60.
        /// </summary>
        public int SessionTimeout { get; set; }

        /// <summary>
        /// The serialization provider to use for session objects
        /// </summary>
        public IValueSerializer SessionSerializer { get; set; }
        #endregion


        /// <summary>
        /// Configure the redis session state provider. Note this is global,
        /// and typically would be called using PreApplicationStartMethodAttribute
        /// (see http://haacked.com/archive/2010/05/16/three-hidden-extensibility-gems-in-asp-net-4.aspx)
        /// </summary>
        /// <param name="config"></param>
        public static void Configure(Action<RedisSessionStateConfiguration> config)
        {
            if (Instance == null) Instance = new RedisSessionStateConfiguration();
            config.Invoke(Instance);
        }

        /// <summary>
        /// Loads the current configuration. If no configuration has been supplied yet,
        /// the settings are initialized by <see cref="UseWebConfig">reading from web.config</see>.
        /// </summary>
        /// <returns></returns>
        public static RedisSessionStateConfiguration GetConfiguration()
        {
            if (Instance == null) UseWebConfig();
            return Instance;
        }

        /// <summary>
        /// Configure the redis session state provider using the regular sessionState
        /// section in web.config.
        /// </summary>
        public static void UseWebConfig()
        {
            Configure(x => x.LoadFromWebConfig());
        }

        /// <summary>
        /// Configures <see cref="SessionSerializer"/> to use the <see cref="ClrBinarySerializer"/>
        /// </summary>
        public void SetSerializationBinary()
        {
            SessionSerializer = new ClrBinarySerializer();
        }

        /// <summary>
        /// Configures <see cref="SessionSerializer"/> to use the <see cref="SSJsonSerializer"/> (ServiceStack.Text JSON)
        /// </summary>
        public void SetSerializationJson()
        {
            SessionSerializer = new SSJsonSerializer();
        }

        /// <summary>
        /// Configures <see cref="SessionSerializer"/> to use the <see cref="SSTypeSerializer"/> (ServiceStack.Text TypeSerializer).
        /// This type is more compact than regular JSON, and can handle more complex objects. Although it looks
        /// similar to JSON, it is not compatible with JSON deserializers.
        /// See https://github.com/ServiceStack/ServiceStack.Text/wiki/JSV-Format for more details.
        /// </summary>
        public void SetSerializationJsv()
        {
            SessionSerializer = new SSTypeSerializer();
        }
        
        /// <summary>
        /// Loads settings from the regular sessionState section in web.config.
        /// </summary>
        public void LoadFromWebConfig()
        {
            // Get the configuration section and set timeout and CookieMode values.
            Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            var config = (SessionStateSection)webConfig.GetSection("system.web/sessionState");

            var stateConnection = config.StateConnectionString;

            if (!String.IsNullOrWhiteSpace(stateConnection))
            {
                var stateConnectionParts = stateConnection.Split('=', ':');
                Host = stateConnectionParts.ElementAtOrDefault(1) ?? "localhost";
                var portAsString = stateConnectionParts.ElementAtOrDefault(2) ?? "6379";
                Port = Int32.Parse(portAsString);
            }

            SessionTimeout = (int)config.Timeout.TotalMinutes;

            // default to binary serialization
            SetSerializationBinary();
        }
        
        /// <summary>
        /// Event raised when there is a deserialization error
        /// </summary>
        public event EventHandler<DeserializationErrorEventArgs> DeserializationError;

        internal void RaiseDeserializationError(object sender, DeserializationErrorEventArgs e)
        {
            DeserializationError(sender, e);
        }

    }
}
