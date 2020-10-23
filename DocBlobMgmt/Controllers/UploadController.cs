using Ionic.Zip;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.DataAccess;

namespace WebApplication.Controllers
{
    public class UploadController : Controller
    {
        public void init()
        {
            // sample insert machanism
            using (var context = new DatabaseContext())
            {
                var docStatusView = new DocStatusView
                {
                    PropertyId = Guid.NewGuid().ToString(),
                    Agreement = false,
                    Appraisal = true,
                    SiteMap = false,
                    Resume = true,
                    Paperwork = true
                };
                context.DocStatusViews.Add(docStatusView);
                context.SaveChanges();
            }
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Upload Home Page";
            return View();
        }

        public DocStatusView GetDocStatusViewByPropertyId(string PropertyId, DatabaseContext context)
        {
            return context.DocStatusViews.ToList().Where(x => x.PropertyId == PropertyId).First();
        }

        // In real enviroment file would be different for each doc type, but it is the same file here for now.
        public static byte[] EncryptFile(string filePath, string password)
        {
            using (ZipFile zip = new ZipFile(filePath))
            {
                // encrypt using password
                zip.Password = password;
                zip.AddFile(filePath);
                using (var streamwriter = new MemoryStream())
                {
                    zip.Save(streamwriter);
                    return streamwriter.GetBuffer();
                }
            }
        }

        public static string GenerateSecretKey(string docId, string propertyId)
        {
            return docId.Take(8) + propertyId.Substring(propertyId.Length - 8);
        }

        private void AddDocuments(DocStatusView docStatusView, DatabaseContext context, string testDoc, string docType)
        {
            var doc_id = Guid.NewGuid().ToString();
            var password = GenerateSecretKey(doc_id, docStatusView.PropertyId);
            byte[] doc_data = EncryptFile(testDoc, password);

            var document = new Document
            {
                Id = doc_id,
                PropertyId = docStatusView.PropertyId,
                DocType = docType,
                FileName = Path.GetFileName(testDoc),
                DocBlob = doc_data
            };
            context.Documents.Add(document);
        }

        [HandleError]
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file == null || file.ContentLength == 0)
            {
                return RedirectToAction("NotFound", "Error");
            }

            string testDoc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings.Get("testDocPath"));

            // Get file data
            string json = null;
            using (StreamReader r = new StreamReader(file.InputStream))
            {
                json = r.ReadToEnd();
            }

            Dictionary<string, string[]> json_Dictionary = (new JavaScriptSerializer()).Deserialize<Dictionary<string, string[]>>(json);
            if (json_Dictionary == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            var data = json_Dictionary.Values.Count > 0 ? json_Dictionary.Values.ElementAt(0) : null;
            if (data == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            using (var context = new DatabaseContext())
            {
                foreach (var propertyId in data)
                {
                    DocStatusView docStatusView = GetDocStatusViewByPropertyId(propertyId, context);
                    if (docStatusView == null)
                    {
                        return RedirectToAction("NotFound", "Error");
                    }

                    // if Agreement is missing then add into Documents table 
                    if (docStatusView.Agreement == false)
                    {
                        AddDocuments(docStatusView, context, testDoc, "Agreement");
                    }
                    // if Appraisal is missing then add into Documents table 
                    if (docStatusView.Appraisal == false)
                    {
                        AddDocuments(docStatusView, context, testDoc, "Appraisal");
                    }
                    // if SiteMap is missing then add into Documents table 
                    if (docStatusView.SiteMap == false)
                    {
                        AddDocuments(docStatusView, context, testDoc, "SiteMap");
                    }
                    // if Resume is missing then add into Documents table 
                    if (docStatusView.Resume == false)
                    {
                        AddDocuments(docStatusView, context, testDoc, "Resume");
                    }
                    // if Paperwork is missing then add into Documents table 
                    if (docStatusView.Paperwork == false)
                    {
                        AddDocuments(docStatusView, context, testDoc, "Paperwork");
                    }
                }
                context.SaveChanges();
            }

            // Show added documents ...
            return RedirectToAction("Index", "Download");
        }
    }
}
