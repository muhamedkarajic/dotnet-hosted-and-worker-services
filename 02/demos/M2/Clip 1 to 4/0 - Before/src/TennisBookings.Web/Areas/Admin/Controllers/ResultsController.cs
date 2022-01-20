using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TennisBookings.ResultsProcessing;

namespace TennisBookings.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ResultsController : Controller
    {
        private readonly IResultProcessor _resultProcessor;
        private readonly ILogger<ResultsController> _logger;

        public ResultsController(IResultProcessor resultProcessor, ILogger<ResultsController> logger)
        {
            _resultProcessor = resultProcessor;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult UploadResults()
        {
            return View();
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> FileUpload(IFormFile file)
        {
            var sw = Stopwatch.StartNew();

            if (file is object && file.Length > 0)
            {
                var fileName = Path.GetTempFileName(); // Upload to a temp file path

                try
                {
                    await using var stream = new FileStream(fileName, FileMode.Create);

                    await file.CopyToAsync(stream);

                    stream.Position = 0;

                    await _resultProcessor.ProcessAsync(stream);
                }
                finally
                {
                    System.IO.File.Delete(fileName); // Delete the temp file
                }
            }

            sw.Stop();

            _logger.LogInformation($"Time taken for result upload and processing " +
                $"was {sw.ElapsedMilliseconds}ms.");

            return RedirectToAction("UploadComplete");
        }

        [HttpGet("FileUploadComplete")]
        public IActionResult UploadComplete()
        {
            return View();
        }

        [HttpGet("FileUploadFailed")]
        public IActionResult UploadFailed()
        {
            return View();
        }
    }
}
