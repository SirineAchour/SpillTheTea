using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Mozilla;
using UnamedWebsite.Models;

namespace UnamedWebsite.Controllers
{
    public class UnamedWebsiteService
    {
        static public bool SearchCheck { get; set; }
        public string ConnectionString { get; set; }

        public UnamedWebsiteService(string connectionString)
        {
            UnamedWebsiteService.SearchCheck = false;
            this.ConnectionString = connectionString;
            // create tables if they dont already exist
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                List<MySqlCommand> cmds = new List<MySqlCommand>();

                cmds.Add(new MySqlCommand(
                    "CREATE TABLE IF NOT EXISTS posts( Id INT(6) UNSIGNED AUTO_INCREMENT, Content VARCHAR(2000) NOT NULL, Email VARCHAR(50), Tags VARCHAR(100),  Hearts INT(6) DEFAULT 0, Reply VARCHAR(2000) DEFAULT NULL, creation_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, PRIMARY KEY(Id))",
                    conn
                    ));
                cmds.Add(new MySqlCommand(
                    "CREATE TABLE IF NOT EXISTS comments ( Id INT(6) UNSIGNED AUTO_INCREMENT, PostId INT(6) UNSIGNED, Content VARCHAR(1000) NOT NULL, Hearts INT(4) DEFAULT 0, creation_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (PostId) REFERENCES posts(Id), PRIMARY KEY(Id,PostId))",
                    conn
                    ));
                /*cmds.Add(new MySqlCommand(
                    "INSERT INTO comments VALUES (null,1,'contennnttt',3,null)",
                    conn
                    ));*/
                foreach (var cmd in cmds)
                {
                    using (var reader = cmd.ExecuteReader()) ;
                }
            }
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public async Task<IEnumerable<Confession>> GetConfessions()
        {
            List<Confession> confessions = new List<Confession>();
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from posts where posts.Reply is null order by creation_time desc", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            confessions.Add(new Confession()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Content = reader["Content"].ToString(),
                                Email = reader["Email"].ToString(),
                                Hearts = Convert.ToInt32(reader["Hearts"]),
                                Tags = reader["Tags"].ToString(),
                                CreationTime = reader["creation_time"].ToString()
                            });
                        }
                    }
                }
            });
            return confessions;
        }
        
        public  async Task<IEnumerable<Advice>> GetAdvice()
        {
            List<Advice> advice = new List<Advice>();

            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from posts where posts.Reply is not null order by creation_time desc", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            advice.Add(new Advice()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Content = reader["Content"].ToString(),
                                Email = reader["Email"].ToString(),
                                Hearts = Convert.ToInt32(reader["Hearts"]),
                                Tags = reader["Tags"].ToString(),
                                CreationTime = reader["creation_time"].ToString(),
                                MainReply = reader["Reply"].ToString()
                            });
                        }
                    }
                }
            });
            return advice;
        }

        public async Task<Advice> AskForAdvice(Advice post)
        {
            // testi 3al string passed to change e table wl e stuff
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO posts VALUES (null,'"+post.Content+"','"+post.Email+"','"+post.Tags+"',0,'',null)", conn);
                    using (var reader = cmd.ExecuteReader()) ;
                }
            });
            return post;
        }

        public async Task<Confession> PostConfession(Confession post)
        {
            // testi 3al string passed to change e table wl e stuff
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO posts VALUES (null,'" + post.Content + "','" + post.Email + "','" + post.Tags + "',0,null,null)", conn);
                    using (var reader = cmd.ExecuteReader()) ;
                }
            });
            return post;
        }
        
        public async Task HeartPost(Confession post)
        {
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE posts SET posts.Hearts = posts.Hearts+1 WHERE posts.Id ="+post.Id, conn);
                    using (var reader = cmd.ExecuteReader()) ;
                }
            });
        }
        
        public async Task Comment(Comment comment)
        {
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO comments VALUES (null,"+comment.PostId+",'"+comment.Content+ "',0,null)", conn);
                    using (var reader = cmd.ExecuteReader());
                }
            });
        }
        
        public async Task HeartComment(Comment comment)
        {
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE comments SET comments.Hearts = comments.Hearts+1 WHERE comments.Id =" + comment.Id+" AND comments.PostId = "+comment.PostId, conn);
                    using (var reader = cmd.ExecuteReader()) ;
                }
            });
        }
        
  /*      public async Task<IEnumerable<Comment>> GetComments(int postId)
        {
            List<Comment> comments = new List<Comment>();
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from comments where comments.PostId="+postId, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new Comment()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Content = reader["Content"].ToString(),
                                Hearts = Convert.ToInt32(reader["Hearts"]),
                                PostId = Convert.ToInt32(reader["Id"])
                            });
                        }
                    }
                }
            });
            return comments;
        }
    */    
        public async Task<IEnumerable<Confession>> SearchConfs(bool by_tags_or_content,string compare) // search by tags ==> true
        {
            var result = new List<Confession>();            
            var queryish = "";

            if (by_tags_or_content) { 
                var tagarray = compare.Split("#");
                foreach (var tag in tagarray)
                {
                    if (tag.Trim() != "")
                        queryish = queryish + "posts.Tags LIKE '%#" + tag.Trim() + "#%' AND ";
                }
                queryish = queryish + "1";
            }
            else
            {
                queryish = "posts.Content LIKE '%" + compare + "%'";
            }
                queryish = queryish + " AND posts.Reply is null";
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from posts where " + queryish + " order by creation_time desc", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Confession()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Content = reader["Content"].ToString(),
                                Email = reader["Email"].ToString(),
                                Hearts = Convert.ToInt32(reader["Hearts"]),
                                Tags = reader["Tags"].ToString(),
                                CreationTime = reader["creation_time"].ToString()
                            });
                        }
                    }
                }
            });
            return result;
        }

        public async Task<IEnumerable<Advice>> SearchAdvice(bool by_tags_or_content, string compare) // search by tags ==> true
        {
            var result = new List<Advice>();
            var queryish = "";

            if (by_tags_or_content)
            {
                var tagarray = compare.Split("#");
                foreach (var tag in tagarray)
                {
                    if (tag.Trim() != "")
                        queryish = queryish + "posts.Tags LIKE '%#" + tag.Trim() + "#%' AND ";
                }
                queryish = queryish + "1";
            }
            else
            {
                queryish = "posts.Content LIKE '%" + compare + "%'";
            }
                queryish = queryish + " AND posts.Reply is not null";
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from posts where " + queryish + " order by creation_time desc", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Advice()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Content = reader["Content"].ToString(),
                                Email = reader["Email"].ToString(),
                                Hearts = Convert.ToInt32(reader["Hearts"]),
                                Tags = reader["Tags"].ToString(),
                                CreationTime = reader["creation_time"].ToString()
                            });
                        }
                    }
                }
            });
            return result;
        }

        public async Task<Confession> GetConfession(int id)
        {
            Confession confession = new Confession();
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from posts where posts.Id = " + id + " AND posts.Reply is null", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            confession.Id = Convert.ToInt32(reader["Id"]);
                            confession.Content = reader["Content"].ToString();
                            confession.Email = reader["Email"].ToString();
                            confession.Hearts = Convert.ToInt32(reader["Hearts"]);
                            confession.Tags = reader["Tags"].ToString();
                            confession.CreationTime = reader["creation_time"].ToString();
                        }
                    }
                }
            });
            return confession;
        }
        
        public async Task<Advice> GetAdvice(int id)
        {
            Advice a = new Advice();
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from posts where posts.Id = " + id+" AND posts.Reply is not null", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            a.Id = Convert.ToInt32(reader["Id"]);
                            a.Content = reader["Content"].ToString();
                            a.Email = reader["Email"].ToString();
                            a.Hearts = Convert.ToInt32(reader["Hearts"]);
                            a.Tags = reader["Tags"].ToString();
                            a.MainReply = reader["Reply"].ToString();
                            a.CreationTime = reader["creation_time"].ToString();
                        }
                    }
                }
            });
            return a;
        }

        public async Task<List<Comment>> GetComments(string post_id)
        {
            List<Comment> comments = new List<Comment>();
            await Task.Run(() =>
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM comments WHERE comments.PostId = "+post_id.Trim() + " order by creation_time desc", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new Comment()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Content = reader["Content"].ToString(),
                                Hearts =  Convert.ToInt32(reader["Hearts"]),
                                PostId = Convert.ToInt32(post_id.Trim())
                            });
                        }
                    }
                }
            });
            return comments;
        }
        /**
         * heart
         * comment
         * view comments
         * search by tags
         * seach by content
         * pots confession 
         * post comment
         * ask for advice
         * heart comment 
         * view confessions 
         * view advice posts
         **/
    }


}
