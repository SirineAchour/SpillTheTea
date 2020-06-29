using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using UnamedWebsite.Models;

namespace UnamedWebsite.Pages
{
    public class ConfessModel : PageModel
    {
        public Confession conf { get; set; }
        public void OnGet()
        {

        }

        public async Task OnGetConfessAsync(Confession form)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync("https://localhost:44310/tea/confession",
                    new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json"))) ;
            }
        }
    }
}