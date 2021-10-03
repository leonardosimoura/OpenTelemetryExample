using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndEndereco.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnderecoController : ControllerBase
    {
        private ILogger<EnderecoController> _logger { get; }
        private IMongoCollection<Endereco> _collection { get; }

        public EnderecoController(ILogger<EnderecoController> logger, IMongoDatabase database)
        {
            _logger = logger;
            _collection = database.GetCollection<Endereco>(typeof(Endereco).Name.ToLower());
        }

        [HttpGet("{id:guid}")]
        public IActionResult Get([FromRoute] Guid id)
        {
            return Ok(_collection.AsQueryable().FirstOrDefault(p => p.EnderecoId == id));
        }

        [HttpGet("por-pessoa/{id:guid}")]
        public IActionResult GetPorPessoa([FromRoute] Guid id)
        {
            return Ok(_collection.AsQueryable().Where(p => p.PessoaId == id).ToList());
        }

        [HttpPost]
        public IActionResult Post([FromBody] Endereco model)
        {
            model.EnderecoId = Guid.NewGuid();
            _collection.InsertOne(model);
            return CreatedAtAction(nameof(Get), new { id = model.EnderecoId }, model.EnderecoId);
        }
    }
}
