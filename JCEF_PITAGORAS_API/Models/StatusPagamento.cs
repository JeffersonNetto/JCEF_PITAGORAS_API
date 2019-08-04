﻿using MongoDB.Bson.Serialization.Attributes;

namespace WebApiCoreMongoDb.Models
{
    public class StatusPagamento
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
    }
}