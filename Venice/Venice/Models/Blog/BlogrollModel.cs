using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Venice.DTO;

namespace Venice.Models.Blog
{
    public class BlogrollModel
    {
        public List<PostDto> Posts { get; set; }
    }
}