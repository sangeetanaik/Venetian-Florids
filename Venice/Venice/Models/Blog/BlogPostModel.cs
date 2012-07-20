using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Venice.DTO;

namespace Venice.Models.Blog
{
    public class BlogPostModel
    {
        public PostDto Post { get; set; }
        public List<CommentDto> Comments { get; set; }
        public CommentDto NewComment { get; set; }

        public BlogPostModel()
        {
            Post = new PostDto();
            Comments = new List<CommentDto>();
            NewComment = new CommentDto();
        }
    }
}