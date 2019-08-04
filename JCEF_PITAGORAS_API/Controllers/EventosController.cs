using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreMongoDb.Models;

namespace WebApiCoreMongoDb.Controllers
{
    [Produces("application/json")]
    [Route("api/Eventos")]
    public class EventosController : Controller
    {
        private Context context;

        public EventosController()
        {
            context = new Context();
        }

        // GET: api/Eventos
        [HttpGet]
        //public async Task<List<Evento>> Get()
        //{
        //    return await context.Eventos.Find(_ => true).ToListAsync();
        //}
        public List<Evento> Get()
        {
            return context.Eventos.Find(_ => _.qtdVagas > 0).ToList();
        }

        // GET: api/Eventos/1
        [HttpGet("{id}")]
        public async Task<List<Evento>> Get(int id)
        {
            return await context.Eventos.Find(_ => _.TipoEvento.Codigo == id).ToListAsync();
        }

        // GET: api/eventos/dia/1
        [HttpGet("dia/{diaDaSemana}")]
        public async Task<List<Evento>> GetEventos(int diaDaSemana)
        {
            return await context.Eventos.Find(_ => _.DiaDaSemana == diaDaSemana).ToListAsync();
        }

        // GET: api/Eventos/Tipos
        [HttpGet]
        [Route("Tipos")]
        public async Task<List<TipoEvento>> GetTipos()
        {
            return await context.TipoEvento.Find(_ => true).ToListAsync();
        }
    }
}