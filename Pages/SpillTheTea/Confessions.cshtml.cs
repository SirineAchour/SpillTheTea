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
    public class ConfessionsModel : PageModel
    {
        public IEnumerable<Confession> Confs { get; set; }
        private UnamedWebsiteService w;
        public List<Comment> Comments { get; set; }

        public ConfessionsModel(UnamedWebsiteService w)
        {
            this.w = w;
            this.Comments = new List<Comment>();
            this.Confs = new List<Confession>();
        }
        public async Task OnGet()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44310/tea/confessions"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    this.Confs = JsonConvert.DeserializeObject<List<Confession>>(apiResponse);
                }
            }

        }

        public async Task OnGetSearchAsync(ForBodyString tags)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync("https://localhost:44310/tea/search/confessions/tags",
                    new StringContent(JsonConvert.SerializeObject(new ForBodyString() { search = tags.search }), Encoding.UTF8, "application/json")))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    this.Confs = JsonConvert.DeserializeObject<List<Confession>>(apiResponse);
                }
            }
        }

        public async Task OnGetSearchContentAsync(ForBodyString content)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync("https://localhost:44310/tea/search/confessions/content",
                    new StringContent(JsonConvert.SerializeObject(new ForBodyString() { search = content.search }), Encoding.UTF8, "application/json")))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    this.Confs = JsonConvert.DeserializeObject<List<Confession>>(apiResponse);
                }
            }
        }
    }
}