using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnamedWebsite.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        //public DateTime PublishDate { get; set; }
        public string Content { get; set; }
        public int Hearts { get; set; }
    }
}
