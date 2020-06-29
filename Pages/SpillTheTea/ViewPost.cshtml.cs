using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using UnamedWebsite.Models;
using System.Net.Http;
using Newtonsoft.Json;
using UnamedWebsite.Controllers;
using System.Text;

namespace UnamedWebsite.Pages
{

    public class ViewPostModel : PageModel
    {
        public int id { get; set; }
        public List<Comment> Comments { get; set; }
        public Advice Post { get; set; }

        public async Task OnGetAsync([FromQuery] string type,[FromQuery] int id)
        {// type is either confession or advice
            this.id = id;
            this.Comments = new List<Comment>();
            this.Post = new Advice();
            await this.GetPost(type, id);
        }
        public async Task GetPost(string type,int id)
        {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:44310/tea/" + type + "/view?id=" + id))
                    {
                        string apiResponse1 = await response.Content.ReadAsStringAsync();
                        this.Post = JsonConvert.DeserializeObject<Advice>(apiResponse1);
                    }

                    using (var response = await httpClient.PostAsync("https://localhost:44310/tea/comments",
                        new StringContent("{\"search\": \"" + id + "\"}", Encoding.UTF8, "application/json")))
                    {
                        string apiResponse2 = await response.Content.ReadAsStringAsync();
                        this.Comments = JsonConvert.DeserializeObject<List<Comment>>(apiResponse2);
                    }
                }
        }
        
        public async Task OnGetCommentAsync(Comment com)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync("https://localhost:44310/tea/comment",
                    new StringContent(JsonConvert.SerializeObject(com), Encoding.UTF8, "application/json"))) ;
            }
        }

        public async Task OnGetLikeAsync(Confession c)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync("https://localhost:44310/tea/post/heart",
                    new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json")));
            }

            await this.GetPost(c.Content, c.Id);
        }

        public async Task OnGetLikeCommentAsync(Comment c)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync("https://localhost:44310/tea/comment/heart",
                    new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json")));
            }

            await this.GetPost(c.Content, c.PostId);
        }
    }
}