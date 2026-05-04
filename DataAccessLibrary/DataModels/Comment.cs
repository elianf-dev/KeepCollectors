using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataModels
{
    public class Comment
    {
        public int CommentId { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
