using MongoDB.Bson.Serialization.Attributes;

namespace WebApiCoreMongoDb.Models
{
    public class Categoria
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public int Codigo { get; set; }

        public string Descricao { get; set; }                

        public decimal Valor { get; set; }
    }
}