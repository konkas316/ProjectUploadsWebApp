using ProjectUploads.Models;
using ProjectUploads.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProjectUploads.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {

        private IUploadRepository _uploadRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;

        public HomeController(IUploadRepository uploadRepository,
             IHostingEnvironment hostingEnvironment,
             ILogger<HomeController> logger)
        {


            _uploadRepository = uploadRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;

        }


        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _uploadRepository.GetAllUploads();
            return View(model);
        }

        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");

            Upload upload = _uploadRepository.GetUpload(id.Value);

            if (upload == null)
            {
                Response.StatusCode = 404;
                return View("UploadNotFound", id.Value);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Upload = upload,
                PageTitle = "Upload Details"
            };

            return View(homeDetailsViewModel);
        }


        [HttpGet]

        public ViewResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(UploadCreateViewModel model)
        {
            if (ModelState.IsValid)

            {
                string uniqueFileName = null;

                if (model.Project != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "projects");
                    uniqueFileName = model.Project.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Project.CopyTo(fileStream);
                    }


                }
                Upload newUpload = new Upload
                {
                    Name = model.Name,
                    Description = model.Description,
                    FileExtention = Path.GetExtension(model.Name),
                    Size = (model.Length / 1024),// For get size in KB
                    Icon = GetIconPath(Path.GetExtension(model.Name)),
                    UploadPath = uniqueFileName
                };

                _uploadRepository.Add(newUpload);
                return RedirectToAction("details", new { id = newUpload.Id });
            }

            return View();
        }

        private string GetIconPath(string fileExtention)
        {
            string Iconpath = "/icons";
            string ext = fileExtention.ToLower();

            switch (ext)
            {
                case ".txt":
                    Iconpath += "/txt.png";
                    break;
                case ".doc":
                case ".docx":
                    Iconpath += "/word.png";
                    break;
                case ".xls":
                case ".xlsx":
                    Iconpath += "/xls.png";
                    break;
                case ".pdf":
                    Iconpath += "/pdf.png";
                    break;
                case ".rar":
                    Iconpath += "/rar.png";
                    break;
                case ".zip":
                case ".7z":
                    Iconpath += "/zip.png";
                    break;
                default:
                    Iconpath += "/unknown.png";
                    break;
            }

            return Iconpath;
        }


        /// How to display a default image in case the source does not exists


        //public static class HtmlHelperExtensions
        //{
        //    public static string ImageOrDefault(this HtmlHelper helper, string filename)
        //    {
        //        var imagePath = uploadsDirectory + filename + ".png";
        //        var imageSrc = File.Exists(HttpContext.Current.Server.MapPath(imagePath))
        //                           ? imagePath : defaultImage;

        //        return imageSrc;
        //    }

        //    private static string defaultImage = "/Content/DefaultImage.png";
        //    private static string uploadsDirectory = "/Content/uploads/";
        //}

        //<img src = "@Html.ImageOrDefault(item.ID)" />

        ////////////////////////////////////////////////////////////////////////////////////////
        ///
        //private string GetIconForFile(string FileName)
        //{
        //    string extension = Path.GetExtension(FileName);
        //    extension = extension.Trim('.').ToLower();
        //    return "~/icons/" + extension + ".png";
        //}

        [HttpGet]

        public ViewResult Edit(int id)
        {
            Upload upload = _uploadRepository.GetUpload(id);
            UploadEditViewModel uploadEditViewModel = new UploadEditViewModel
            {
                Id = upload.Id,
                Name = upload.Name,
                Description = upload.Description,
                //Department = upload.Department,
                ExistingUploadPath = upload.UploadPath
            };
            return View(uploadEditViewModel);
        }


        [HttpPost]
        public IActionResult Edit(UploadEditViewModel model)
        {

            if (ModelState.IsValid)
            {

                Upload upload = _uploadRepository.GetUpload(model.Id);

                upload.Name = model.Name;
                upload.Description = model.Description;
                //upload.Department = model.Department;

                if (model.Project != null)
                {

                    if (model.ExistingUploadPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "projects", model.ExistingUploadPath);
                        System.IO.File.Delete(filePath);
                    }

                    upload.UploadPath = ProcessUploadedFile(model);
                }


                _uploadRepository.Update(upload);

                return RedirectToAction("index");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Delete(UploadEditViewModel model)
        {

            Upload upload = _uploadRepository.GetUpload(model.Id);
            if (upload != null)
            {
                _uploadRepository.Delete(upload.Id);

                if (upload.UploadPath != null)
                {
                    string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "projects", upload.UploadPath);
                    System.IO.File.Delete(filePath);
                }
            }
            return RedirectToAction("index");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Download(UploadEditViewModel model)
        {
            Upload upload = _uploadRepository.GetUpload(model.Id);

            if (upload.UploadPath == null)

                return View("NotFound");


            if (upload.UploadPath != null)
            {
                string fileName = Path.GetFileName(upload.UploadPath);
                string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                    "projects", upload.UploadPath);
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/force-download", fileName);

            }

            return View(model);
        }

        //        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        //        {
        //            long size = files.Sum(f => f.Length);

        //            foreach (var formFile in files)
        //            {
        //                if (formFile.Length > 0)
        //                {
        //                    var filePath = Path.GetTempFileName();

        //                    using (var stream = System.IO.File.Create(filePath))
        //                    {
        //                        await formFile.CopyToAsync(stream);
        //                    }
        //                }
        //            }

        //            // Process uploaded files
        //            // Don't rely on or trust the FileName property without validation.

        //            return Ok(new { count = files.Count, size, filePath });
        //        }

        //        public IActionResult Get(IFileProvider fileProvider)
        // {
        //      IDirectoryContents contents = fileProvider.GetDirectoryContents("wwwroot/updates");

        //      var lastUpdate =
        //                contents.ToList()
        //                .OrderByDescending(f => f.LastModified)
        //                .FirstOrDefault();

        //      return Ok(lastUpdate?.Name);
        // }


        private string ProcessUploadedFile(UploadCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Project != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "projects");
                uniqueFileName = model.Project.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Project.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }



    }
}
