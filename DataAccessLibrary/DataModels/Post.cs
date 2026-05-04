using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataModels
{
    public class Post
    {
        public int PostId { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string? Content { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
    }
}
