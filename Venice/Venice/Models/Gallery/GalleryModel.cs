using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Venice.DTO;

namespace Venice.Models.Gallery
{
    public class GalleryModel
    {
        public GalleryDto Gallery { get; set; }

        public List<ImagesDto> Images { get; set; }

        public GalleryModel()
        {
            Gallery = new GalleryDto();
            Images = new List<ImagesDto>();
        }
    }
}