using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApiCoreMongoDb.Models
{
    public class Inscricao
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }        
        public string PaymentCode { get; set; }
        public string TransactionId { get; set; }
        public string DataPagamento { get; set; }
        public Categoria Categoria { get; set; }
        public StatusPagamento StatusPagamento { get; set; }      
        public List<Evento> Eventos { get; set; }  
    }
}
