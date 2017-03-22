using System;
namespace PalestraMongoDB.Repository
{
	public interface IConnectionStringConfiguration
	{
		string ConnectionString { get; }
	}
}