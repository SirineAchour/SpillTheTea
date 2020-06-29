using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnamedWebsite.Models;

namespace UnamedWebsite.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TeaController : ControllerBase
    {
        public TeaController(UnamedWebsiteService d)
        {
            this.UWS = d;
        }

        public UnamedWebsiteService UWS { get; set; }

        [Route("dummy")]
        [HttpGet]
        public async Task<string> DummyFunc()
        {
            return "heyyyy";
        }

        [Route("confessions")]
        [HttpGet]
        public async Task<IEnumerable<Confession>> GetConfessions()
        {
            return await this.UWS.GetConfessions();
        }

        [Route("advice_posts")]
        [HttpGet]
        public async Task<IEnumerable<Advice>> GetAdvicePosts()
        {
            return await this.UWS.GetAdvice();
        }

/*        [Route("comments")]
        [HttpGet]
        public async Task<IEnumerable<Comment>> GetComments([FromQuery] int postId)
        {
            return await this.UWS.GetComments(postId);
        }
*/
        [Route("search/confessions/tags")]
        [HttpPost]
        public async Task<IEnumerable<Confession>> SearchConfessionsByTags([FromBody] ForBodyString s)
        {
            return await this.UWS.SearchConfs(true, s.search);
        }

        [Route("search/advice/tags")]
        [HttpPost]
        public  async Task<IEnumerable<Confession>> SearchAdviceByTags([FromBody] ForBodyString s)
        {
            return await this.UWS.SearchAdvice(true, s.search);
        }

        [Route("search/confessions/content")]
        [HttpPost]
        public async Task<IEnumerable<Confession>> SearchConfessionsByContent([FromBody] ForBodyString s)
        {
            return await this.UWS.SearchConfs(false, s.search);
        }

        [Route("search/advice/content")]
        [HttpPost]
        public async Task<IEnumerable<Confession>> SearchAdviceByContent([FromBody] ForBodyString s)
        {
            return await this.UWS.SearchAdvice(false, s.search);
        }

        [Route("confession")]
        [HttpPost]
        public async Task<Confession> PostConfession([FromBody] Confession c)
        {
            return await this.UWS.PostConfession(c);
        }

        [Route("advice")]
        [HttpPost]
        public async Task<Confession> AskForAdvice([FromBody] Advice a)
        {
            return await this.UWS.AskForAdvice(a);
        }

        [Route("comment")]
        [HttpPost]
        public async Task<Comment> PostComment ([FromBody] Comment c)
        {
            await this.UWS.Comment(c);
            return c;
        }

        [Route("post/heart")]
        [HttpPost]
        public async Task HeartPost([FromBody] Confession c) => await this.UWS.HeartPost(c);

        [Route("comment/heart")]
        [HttpPost]
        public async Task HeartComment([FromBody] Comment c) => await this.UWS.HeartComment(c);
        
        [Route("confession/view")]
        [HttpGet]
        public async Task<Confession> GetConfession([FromQuery] int id)
        {
            return await this.UWS.GetConfession(id);
        }

        [Route("advice/view")]
        [HttpGet]
        public async Task<Confession> GetAdvicee([FromQuery] int id)
        {
            return await this.UWS.GetAdvice(id);
        }

        [Route("comments")]
        [HttpPost]
        public async Task<IEnumerable<Comment>> GetComments([FromBody] ForBodyString s)
        {
            return await this.UWS.GetComments(s.search);
        }
        /**
         * do i need to test ken the things im posting are null or not ?
         **/
    }
}
