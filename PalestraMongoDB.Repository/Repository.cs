using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PalestraMongoDB.Domain;

namespace PalestraMongoDB.Repository
{
    public class Repository<T> where T : Entity, new()
	{
		private readonly IMongoClient client;
		private readonly IMongoDatabase db;
		private readonly IMongoCollection<T> collection;

		public Repository(IConnectionStringConfiguration connectionStringConfiguration)
		{
			var mongoURL = MongoUrl.Create(connectionStringConfiguration.ConnectionString);
			client = new MongoClient(mongoURL);
			db = client.GetDatabase(mongoURL.DatabaseName);

			collection = db.GetCollection<T>(typeof(T).Name);
		}

		#region Query Methods
		public async Task<T> Get(string id)
		{
			var cursor = await collection.FindAsync(GetIdFilter(id));
			var result = await cursor.SingleOrDefaultAsync();
			return result;
		}

		public Task<T> Get(Expression<Func<T, bool>> predicate)
		{
			return Task.Factory.StartNew(() => collection.Find(predicate).SingleOrDefault());
		}

		public Task<IQueryable<T>> Load()
		{
			return Task.Factory.StartNew(() =>
			{
				var query = this.collection.Find(new BsonDocument()).ToListAsync();
				return query.Result.AsQueryable();
			});
		}

		public Task<IQueryable<T>> Load(Expression<Func<T, bool>> predicate)
		{
			return Task.Factory.StartNew(() =>
			{
				var query = this.collection.Find(predicate).ToListAsync();
				return query.Result.AsQueryable();
			});
		}

		public Task Delete(string id)
		{
			return Task.Factory.StartNew(() =>
			{
				collection.DeleteOneAsync(GetIdFilter(id)).Wait();
			});
		}

		public Task Update(T entity)
		{
			return Task.Factory.StartNew(() =>
			{
				collection.ReplaceOneAsync(GetIdFilter(entity.Id), entity).Wait();
			});
		}

		public Task Save(T entity)
		{
			return Task.Factory.StartNew(() =>
			{
				collection.InsertOneAsync(entity).GetAwaiter().GetResult();
			});
		}
		#endregion

		#region Private Methods
		private FilterDefinition<T> GetIdFilter(string id)
		{
			return Builders<T>.Filter.Eq(entity => entity.Id, ObjectId.Parse(id));
		}

		private FilterDefinition<T> GetIdFilter(ObjectId id)
		{
			return Builders<T>.Filter.Eq(entity => entity.Id, id);
		}
		#endregion
	}
}