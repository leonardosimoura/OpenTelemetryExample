using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd
{

    public class ListagemPessoaViewModel
    {
        public Guid PessoaId { get; set; }
        public string Nome { get; set; }
        public IEnumerable<Endereco> Enderecos { get; set; } = Enumerable.Empty<Endereco>();
    }

    public class Pessoa
    {
        public Guid PessoaId { get; set; }
        public string Nome { get; set; }
    }

    public class Endereco
    {
        public Guid EnderecoId { get; set; }
        public Guid PessoaId { get; set; }
        public string Nome { get; set; }
    }
}
