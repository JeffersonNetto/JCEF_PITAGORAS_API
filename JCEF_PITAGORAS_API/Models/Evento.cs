using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebApiCoreMongoDb.Models
{
    public class Evento
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public int Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string DataHora { get; set; }
        public int DiaDaSemana { get; set; }
        public int qtdVagas { get; set; }
        public virtual TipoEvento TipoEvento { get; set; }
        public virtual List<Palestrante> Palestrantes { get; set; }
    }
}
