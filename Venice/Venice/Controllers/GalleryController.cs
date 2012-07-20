using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Venice.DTO;
using Venice.Data;
using Venice.Infrastructure;
using Venice.Models.Gallery;

namespace Venice.Controllers
{
    public class GalleryController : Controller
    {
        public ActionResult Index()
        {
            var model = GalleryQueries.GetAllGalleries();
            return View(model);
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult View(int id)
        {
            var model = new GalleryModel
            {
                Gallery = GalleryQueries.GetGalleryById(id),
                Images = GalleryQueries.GetAllImagesByGalleryId(id)
            };
            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var model = new GalleryModel
                            {
                                Gallery = GalleryQueries.GetGalleryById(id),
                                Images = GalleryQueries.GetAllImagesByGalleryId(id)
                            };
            return View(model);
        }
        

        [HttpPost]
        [Authorize]
        public ActionResult CreateGallery(GalleryDto model)
        {
            if (model != null)
            {
                var dto = model;
                dto.CreatedBy = "Authorized User";
                dto.CreatedOn = DateTime.Now;
               var galleryId = GalleryQueries.CreateGallery(dto);
               return RedirectToAction("Edit", new { id = galleryId });
            }
            return View("Create");
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadImage(HttpPostedFileBase UploadedImage, string galleryId)
        {
            if (UploadedImage == null)
                throw new FileNotFoundException("No image supplied, please try again.");

            var message = string.Empty;
            
            if (UploadedImage.ContentType != "image/pjpeg" && UploadedImage.ContentType != "image/jpeg" && UploadedImage.ContentType != "image/png" && UploadedImage.ContentType != "image/gif")
            {
                message = "Not a valid file type.";
            }
            else if (UploadedImage.ContentLength > Constants.ImageSize)
            {
                message = "File size is too large, please limit this to 1MB.";
            }
            else
            {
                //var filename = new Guid() + ".png";
                var image = new ImagesDto()
                                {
                                    //Url = filename,
                                    UploadedBy = "Authorized User",
                                    GalleryId =Convert.ToInt32(galleryId)
                                };
                var imageId = GalleryQueries.UploadImage(image);
                if (imageId > 0)
                {
                    var p = Server.MapPath("~") + @"Images\GalleryImages\";
                  new WebImage(UploadedImage.InputStream).Save(p + imageId + ".png", Constants.ImageType);
                    message = "Image uploaded successfully";
                }else
                {
                    message = "There was some problem with image upload. Please try again.";
                }

            }
            TempData["Message"] = message;
            return RedirectToAction("Edit", new { @id = galleryId });
        }

        [Authorize]
        public ActionResult MakeCoverImage(int galleryId, int imageId)
        {
            var result = GalleryQueries.AddCoverImage(galleryId, imageId);
            TempData["Message"] = result > 0
                                      ? "Cover image successfully added"
                                      : "There was some problem adding the cover image. Please try again.";

            return RedirectToAction("Edit", new { @id = galleryId });
        }

        [Authorize]
        public ActionResult RemoveImage(int galleryId, int imageId)
        {
            var imagesToBeRemoved = new List<int> {imageId};
            GalleryQueries.RemoveImagesfromGallery(galleryId, imagesToBeRemoved);
            System.IO.File.Delete(Server.MapPath("~") + @"Images\GalleryImages\" + imageId + ".png");
            TempData["Message"] = "Image successfully removed";

            return RedirectToAction("Edit", new { @id = galleryId });
        }

        [Authorize]
        public ActionResult UpdateImageDetails(int imgId, int galleryId, string imgTitle, string imgDesc)
        {
            var result = GalleryQueries.UpdateImageTitleDesc(imgId, imgTitle, imgDesc);
            TempData["Message"] = result > 0
                                      ? "Image successfully updated"
                                      : "There was some problem updating the image. Please try again.";

            return RedirectToAction("Edit", new { @id = galleryId });
        }
        
    }
}
