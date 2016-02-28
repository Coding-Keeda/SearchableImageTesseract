using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Tesseract;

//# Intruction to install Tesseract wrapper

//1. In your VS .Go to TOOLS --> NuGet Package Manager --> Package Manager Console
//2. Write below line first in the console
//3. < UnInstall-Package Tesseract >  and then
//4. < Install-Package Tesseract > 

//# Following tesseract C# wrapper has been used for this project 
//https://www.nuget.org/packages/Tesseract/


//For any query ,feel free to contact me at amitsb3747@gmail.com

namespace TesseractImageSearch.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult postImage()
        {
           var postedFile= Request.Files[0];
            var subHTML = string.Empty;
            var meanConfidenceLabel = string.Empty;
            dynamic arrTess=null;
            if (postedFile != null && postedFile.ContentLength > 0)
            {
                //If you get error at this line .please download Visual C++ Redistributable for Visual Studio 2015
                //https://www.microsoft.com/en-in/download/details.aspx?id=48145
                //https://github.com/charlesw/tesseract/issues/245
                using (var engine = new TesseractEngine(Server.MapPath(@"~/tessdata"), "eng", EngineMode.Default))
                {
                    // have to load Pix via a bitmap since Pix doesn't support loading a stream.
                    using (var image = new System.Drawing.Bitmap(postedFile.InputStream))
                    {
                        using (var pix = PixConverter.ToPix(image))
                        {
                            using (var page = engine.Process(pix))
                            {
                                meanConfidenceLabel = String.Format("{0:P}", page.GetMeanConfidence());
                                //var text = page.GetText();  //to get text out of the image.
                                var body = page.GetHOCRText(1); //to generate HOCR//
                                //Below code (very basic) to get HTML inside the body tag
                                var totalindex = body.Length;
                                var index = body.LastIndexOf("<body>") + 9;
                                var lastIndex = body.LastIndexOf("</body>");
                                var lengthToTrim = lastIndex - index;
                                subHTML = body.Substring(index, lengthToTrim);

                                arrTess = new { mean = meanConfidenceLabel, html = subHTML };
                            }
                        }
                    }
                }
            }
            return Json(arrTess,JsonRequestBehavior.AllowGet);
        }

    }
}
