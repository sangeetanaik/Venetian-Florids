using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace Venice.ConnectionFactory
{
    /// <summary>
    /// Connection class allows easy access to connection strings from the application config file
    /// Config file keys should be the same as the below strings
    /// </summary>
    public class Connection
    {

        /// <summary>
        /// The class only contains static methods
        /// </summary>
        private Connection() {}

        /// <summary>
        /// Returns a SqlConnection object based on the connection string passed in.
        /// </summary>
        /// <returns>SqlConnection object</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString().ConnectionString);
        }

        /// <summary>
        /// Returns the connection string for the database passed in.
        /// </summary>
        /// <returns>Connection string</returns>
        private static ConnectionStringSettings GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MyDB"];
            if (connectionString == null)
                throw new Exception(string.Format("Connection string {0} not in config", "MyDB"));

            return connectionString;
        }

        public static DbConnection GetOpenDbConnection()
        {
            var connectionString = GetConnectionString();

            // Work out connection string and provider name
            var providerName = String.IsNullOrEmpty(connectionString.ProviderName) ? "System.Data.SqlClient" : connectionString.ProviderName;

            var provider = DbProviderFactories.GetFactory(providerName);
            var connection = provider.CreateConnection();
            connection.Open();

            return connection;
        }
    }
}