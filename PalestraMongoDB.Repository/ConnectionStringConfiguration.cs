using System;
namespace PalestraMongoDB.Repository
{
	public class ConnectionStringConfiguration : IConnectionStringConfiguration
	{
		public string ConnectionString { get; set; }

		public ConnectionStringConfiguration(string connectionString)
		{
			ConnectionString = connectionString;
		}
	}
}
