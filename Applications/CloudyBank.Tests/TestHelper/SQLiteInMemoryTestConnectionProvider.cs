using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Connection;
using FluentNHibernate.Cfg.Db;
using System.Data;

namespace  CloudyBank.UnitTests.TestHelper
{
    public class SQLiteInMemoryTestConnectionProvider : DriverConnectionProvider
    {

        private static IDbConnection _connection;

        public override IDbConnection GetConnection()
        {
            if (_connection == null)
                _connection = base.GetConnection();
            return _connection;
        }

        public override void CloseConnection(IDbConnection conn)
        {
        }

        /// <summary>
        /// Destroys the connection that is kept open in order to 
        /// keep the in-memory database alive.  Destroying
        /// the connection will destroy all of the data stored in 
        /// the mock database.  Call this method when the
        /// test is complete.
        /// </summary>
        public static void ExplicitlyDestroyConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }
    }
}
