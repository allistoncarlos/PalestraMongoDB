using System;
using MongoDB.Bson;

namespace PalestraMongoDB.Domain
{
	public abstract class Entity
	{
		public ObjectId Id { get; set; }
	}
}