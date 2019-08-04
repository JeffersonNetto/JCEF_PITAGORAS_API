using MongoDB.Driver;
using System;
using WebApiCoreMongoDb.Models;

namespace WebApiCoreMongoDb
{
    public class Context
    {
        public static string ConnectionString { get; set; }
        public static string DatabaseName { get; set; }
        public static bool IsSSL { get; set; }
        private IMongoDatabase database { get; }

        public Context()
        {            
            try
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));

                if (IsSSL)               
                    settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                
                var mongoClient = new MongoClient(settings);
                database = mongoClient.GetDatabase(DatabaseName);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível se conectar com o servidor.", ex);
            }
        }

        public IMongoCollection<Post> Posts
        {
            get
            {
                return database.GetCollection<Post>("Posts");
            }
        }

        public IMongoCollection<Evento> Eventos
        {
            get
            {
                return database.GetCollection<Evento>("Eventos");
            }
        }

        public IMongoCollection<Palestrante> Palestrantes
        {
            get
            {
                return database.GetCollection<Palestrante>("Palestrantes");
            }
        }

        public IMongoCollection<Patrocinador> Patrocinadores
        {
            get
            {
                return database.GetCollection<Patrocinador>("Patrocinadores");
            }
        }

        public IMongoCollection<TipoEvento> TipoEvento
        {
            get
            {
                return database.GetCollection<TipoEvento>("TipoEvento");
            }
        }

        public IMongoCollection<Categoria> Categorias
        {
            get
            {
                return database.GetCollection<Categoria>("Categorias");
            }
        }

        public IMongoCollection<Inscricao> Inscricoes
        {
            get
            {
                return database.GetCollection<Inscricao>("Inscricoes");
            }
        }

        public IMongoCollection<StatusPagamento> StatusPagamento
        {
            get
            {
                return database.GetCollection<StatusPagamento>("StatusPagamento");
            }
        }
    }
}
