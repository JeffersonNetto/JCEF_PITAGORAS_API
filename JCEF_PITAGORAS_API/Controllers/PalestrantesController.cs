using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreMongoDb.Models;
using System.Linq;

namespace WebApiCoreMongoDb.Controllers
{
    [Produces("application/json")]
    [Route("api/Palestrantes")]
    public class PalestrantesController : Controller
    {
        private Context context;

        public PalestrantesController()
        {
            context = new Context();
        }

        // GET: api/Palestrantes
        [HttpGet]
        //public async Task<List<Palestrante>> Get()
        //{
        //    var lst = await context.Palestrantes.Find(_ => true).ToListAsync();
        //    return lst.OrderBy(_ => _.Nome).ToList();
        //}

        public List<Palestrante> Get()
        {
            return context.Palestrantes.Find(_ => true).ToList().OrderBy(_ => _.Nome).ToList();
        }

        // GET: api/Palestrantes/1
        [HttpGet("{id}")]
        public async Task<List<Palestrante>> Get(int id)
        {
            return await context.Palestrantes.Find(_ => _.Codigo == id).ToListAsync();
        }
    }
}