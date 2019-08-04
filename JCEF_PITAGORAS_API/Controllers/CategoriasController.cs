using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreMongoDb.Models;

namespace WebApiCoreMongoDb.Controllers
{
    [Produces("application/json")]
    [Route("api/Categorias")]
    public class CategoriasController : Controller
    {
        private Context context;

        public CategoriasController()
        {
            context = new Context();
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<List<Categoria>> Get()
        {
            return await context.Categorias.Find(_ => true).ToListAsync();
        }

        // GET: api/Categorias/1
        [HttpGet("{id}")]
        public async Task<List<Categoria>> Get(int id)
        {
            return await context.Categorias.Find(_ => _.Codigo == id).ToListAsync();
        }
    }
}