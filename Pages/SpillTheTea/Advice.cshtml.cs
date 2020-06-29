using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using UnamedWebsite.Controllers;
using UnamedWebsite.Models;

namespace UnamedWebsite.Pages
{
    public class AdviceModel : PageModel
    {
        public IEnumerable<Advice> advice { get; set; }
        private UnamedWebsiteService w { get; set; }
        public AdviceModel(UnamedWebsiteService w)
        {
            this.w = w;
            this.advice = null;
        }
        public async Task OnGet()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44310/tea/advice_posts"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    this.advice = JsonConvert.DeserializeObject<List<Advice>>(apiResponse);
                }
            }
            
        }

        public async Task OnGetSearchAsync(ForBodyString tags)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync("https://localhost:44310/tea/search/advice/tags",
                    new StringContent(JsonConvert.SerializeObject(new ForBodyString() { search = tags.search }), Encoding.UTF8, "application/json")))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    this.advice = JsonConvert.DeserializeObject<List<Advice>>(apiResponse);
                }
            }
        }

        public async Task OnGetSearchContentAsync(ForBodyString content)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync("https://localhost:44310/tea/search/advice/content",
                    new StringContent(JsonConvert.SerializeObject(new ForBodyString() { search = content.search }), Encoding.UTF8, "application/json")))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    this.advice = JsonConvert.DeserializeObject<List<Advice>>(apiResponse);
                }
            }
        }
    }
}