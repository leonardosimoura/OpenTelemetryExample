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
        public IEnumerable<EnderecoViewModel> Enderecos { get; set; } = Enumerable.Empty<EnderecoViewModel>();
    }

    public class PessoaViewModel
    {
        public Guid PessoaId { get; set; }
        public string Nome { get; set; }
    }

    public class EnderecoViewModel
    {
        public Guid EnderecoId { get; set; }
        public Guid PessoaId { get; set; }
        public string Nome { get; set; }
    }
}
