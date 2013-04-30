using System;
using System.Collections.Generic;
using System.Configuration;
using NHibernate.Connection;
using NHibernate;

namespace CloudyBank.DbTool.Technical
{
    public class DynamicConnectionProvider : DriverConnectionProvider
    {
        public static String CurrentDatabaseName;

        private string _connectionString;

        protected override string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[CurrentDatabaseName].ConnectionString;
            }
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            // Connection string in the configuration overrides named connection string
            if (!settings.TryGetValue(NHibernate.Cfg.Environment.ConnectionString, out _connectionString))
                _connectionString = GetNamedConnectionString(settings);

            if (_connectionString == null)
            {
                throw new HibernateException("Could not find connection string setting (set "
                    + NHibernate.Cfg.Environment.ConnectionString + " or "
                    + NHibernate.Cfg.Environment.ConnectionStringName + " property)");
            }

            ConfigureDriver(settings);
        }
    }
}
