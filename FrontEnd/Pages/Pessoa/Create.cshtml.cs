using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FrontEnd.Pages.Pessoa
{
    public class CreateModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "Nova Pessoa";
        }

        [BindProperty]
        public PessoaViewModel Pessoa { get; set; }

        [BindProperty]
        public IEnumerable<EnderecoViewModel> Enderecos { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var responsePessoa = await httpClient.PostAsync(@"http://localhost:5051/pessoa", new StringContent(JsonConvert.SerializeObject(Pessoa), Encoding.UTF8,  "application/json"));
                responsePessoa.EnsureSuccessStatusCode();
                var pessoaId= JsonConvert.DeserializeObject<Guid>(await responsePessoa.Content.ReadAsStringAsync());

                foreach (var item in Enderecos)
                {
                    item.PessoaId = pessoaId;
                    var responseEndereco = await httpClient.PostAsync($@"http://localhost:5052/endereco", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
                    responseEndereco.EnsureSuccessStatusCode();
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
