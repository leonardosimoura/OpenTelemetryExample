using System;

namespace BackEndEndereco
{
    public class Endereco
    {
        public Guid EnderecoId { get; set; }  
        public Guid PessoaId { get; set; }
        public string Nome { get; set; }
    }
}
