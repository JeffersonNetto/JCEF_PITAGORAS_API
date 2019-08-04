using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreMongoDb.Models;

namespace WebApiCoreMongoDb.Controllers
{
    [Produces("application/json")]
    [Route("api/Patrocinadores")]
    public class PatrocinadoresController : Controller
    {
        private Context context;

        public PatrocinadoresController()
        {
            context = new Context();
        }

        // GET: api/Patrocinadores
        [HttpGet]
        public async Task<List<Patrocinador>> Get()
        {            
            return await context.Patrocinadores.Find(_ => true).ToListAsync();
        }

        // GET: api/Palestrantes/1
        [HttpGet("{id}")]
        public async Task<List<Patrocinador>> Get(int id)
        {
            return await context.Patrocinadores.Find(_ => _.Codigo == id).ToListAsync();
        }
    }
}