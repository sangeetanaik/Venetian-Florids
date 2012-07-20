using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Venice.ConnectionFactory;
using Venice.Data;

namespace Venice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "";// Server.MapPath("~");

            var model = GalleryQueries.GetRandomImages(5);

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
