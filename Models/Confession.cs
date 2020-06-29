using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnamedWebsite.Models
{
    public class Confession
    {
        public int Id { get; set; }
        public string Content { get; set; }
        //public string PublishDate { get; set; }
        public string Tags { get; set; }
        public string Email { get; set; }
        public int Hearts { get; set; }
        public Confession()
        {
            this.Tags = "";
        }
        public override string ToString() => JsonSerializer.Serialize<Confession>(this);
    }
}
