using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Venice.DTO
{
    public class GalleryDto
    {
        public int GalleryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public ImagesDto CoverImage { get; set; }
    }
}