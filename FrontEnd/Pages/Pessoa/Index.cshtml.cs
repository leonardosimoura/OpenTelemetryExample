using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FrontEnd.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FrontEnd.Pages.Pessoa
{
    public class IndexModel : PageModel
    {
        private readonly BackEndOptions _backEnds;
        public IndexModel(ILogger<IndexModel> logger, IOptions<BackEndOptions> backEndsOptions)
        {
            _logger = logger;
            _backEnds = backEndsOptions.Value;
        }

        private ILogger<IndexModel> _logger { get; }
        public IEnumerable<ListagemPessoaViewModel> Pessoas { get; set; } = Enumerable.Empty<ListagemPessoaViewModel>();
        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Title"] = "Pessoas";
            Pessoas = Enumerable.Empty<ListagemPessoaViewModel>();

            using (var httpClient = new HttpClient())
            {
                _logger.LogInformation("buscando pessoas");
                var response = await httpClient.GetAsync(@$"{_backEnds.PessoaUrl}/pessoa");
                response.EnsureSuccessStatusCode();
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    Pessoas = JsonConvert.DeserializeObject<IEnumerable<ListagemPessoaViewModel>>(await response.Content.ReadAsStringAsync());

                foreach (var item in Pessoas)
                {
                    _logger.LogInformation($"buscando endereço -  {item.PessoaId} - {item.Nome}");
                    var responseEndereco = await httpClient.GetAsync(@$"{_backEnds.EnderecoUrl}/endereco/por-pessoa/{item.PessoaId}");
                    responseEndereco.EnsureSuccessStatusCode();
                    if (responseEndereco.StatusCode != System.Net.HttpStatusCode.NoContent)
                        item.Enderecos = JsonConvert.DeserializeObject<IEnumerable<EnderecoViewModel>>(await responseEndereco.Content.ReadAsStringAsync());
                }
            }

            return Page();
        }
    }
}
