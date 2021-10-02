using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEnd.Pages
{
    public class PessoaModel : PageModel
    {
        public IEnumerable<ListagemPessoaViewModel> Pessoas { get; set; } = Enumerable.Empty<ListagemPessoaViewModel>();
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(@"http://localhost:5051/pessoa");
                    Pessoas = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ListagemPessoaViewModel>>(await response.Content.ReadAsStringAsync());

                    foreach (var item in Pessoas)
                    {
                        var responseEndereco = await httpClient.GetAsync($@"http://localhost:5052/endereco/por-pessoa/{item.PessoaId}");
                        if (responseEndereco.StatusCode != System.Net.HttpStatusCode.NoContent)
                        {
                            item.Enderecos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Endereco>>(await responseEndereco.Content.ReadAsStringAsync());

                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return Page();
        }
    }
}
