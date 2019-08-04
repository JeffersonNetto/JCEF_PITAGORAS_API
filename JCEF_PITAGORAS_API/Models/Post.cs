using MongoDB.Bson.Serialization.Attributes;

namespace WebApiCoreMongoDb.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
