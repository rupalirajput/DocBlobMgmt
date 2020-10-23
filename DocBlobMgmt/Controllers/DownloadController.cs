using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.DataAccess;

namespace WebApplication.Controllers
{
    public class DownloadController : Controller
    {
        [HandleError]
        public ActionResult Index()
        {
            ViewBag.Title = "Download Home Page";
            try
            {
                var context = new DatabaseContext();
                return View(context.Documents.ToList());
            }
            catch (Exception ex)
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        [HandleError]
        [HttpGet]
        public object DownloadFile(string Id)
        {
            if(Id == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            var context = new DatabaseContext();
            List<Document> ObjDocs = context.Documents.ToList();

            // get document data by propertyId
            var doc = (from FC in ObjDocs
                       where FC.Id.Equals(Id)
                       select new { FC.Id, FC.PropertyId, FC.FileName, FC.DocBlob }).ToList().First();
            if (doc == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            string testDoc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings.Get("testDocPath"));

            MemoryStream ms = new MemoryStream();
            using (var streamreader = new MemoryStream(doc.DocBlob))
            {
                using (ZipFile zip = ZipFile.Read(streamreader))
                {
                    if(zip.Count == 0)
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                    // Unzip/decrypt file using given password
                    ZipEntry entry;
                    entry = zip[testDoc];
                    entry.ExtractWithPassword(ms, UploadController.GenerateSecretKey(doc.Id, doc.PropertyId));
                }
                return File(ms.GetBuffer(), "application/octet-stream", doc.FileName);
            }
        }
    }
}
