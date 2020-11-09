using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace random_xkcd
{
    public static class FunctionDark
    {
        [FunctionName("xkcddark")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            // Disable caching, (most sites like Github cache markdown and other embedded images)
            req.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
            byte[] bytes = new byte[8];
            Function.Rand.NextBytes(bytes);
            req.HttpContext.Response.Headers.Add("ETag", Convert.ToBase64String(bytes));
            // Fetch the image, and apply a filter on it
            var val = await Function.Fetch();
            val.Mutate(x => x.Grayscale());
            val.Mutate(x => x.Invert());
            // Return image
            var ms = new MemoryStream();
            await val.SaveAsPngAsync(ms);
            ms.Position = 0;
            return new FileStreamResult(ms, "image/png");
        }
    }
}
