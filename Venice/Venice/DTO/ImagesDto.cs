using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Venice.DTO
{
    public class ImagesDto
    {
        public int ImageId { get; set; }
        public int GalleryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public string Url { get; set; }
        public DateTime UploadedOn { get; set; }
        public string UploadedBy { get; set; }
    }
}