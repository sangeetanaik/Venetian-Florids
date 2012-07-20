using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Venice.DTO
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}