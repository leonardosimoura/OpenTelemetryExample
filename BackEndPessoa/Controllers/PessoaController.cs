using BackEndPessoa.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndPessoa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PessoaController : ControllerBase
    {
        private ILogger<PessoaController> _logger { get; }
        private BackEndPessoaContext _dbContext { get; }

        public PessoaController(ILogger<PessoaController> logger, BackEndPessoaContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_dbContext.Pessoa.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] Guid id)
        {
            return Ok(_dbContext.Pessoa.FirstOrDefault(p => p.PessoaId == id));
        }

        [HttpPost]
        public IActionResult Post([FromBody]  Pessoa model)
        {
            model.PessoaId = Guid.NewGuid();
             _dbContext.Pessoa.Add(model);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = model.PessoaId }, model.PessoaId);
        }
    }
}
