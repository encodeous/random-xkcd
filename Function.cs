using System;
using System.Buffers.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace random_xkcd
{
    public static class Function
    {
        public static Random Rand = new Random();
        [FunctionName("xkcd")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            // Disable caching, (most sites like Github cache markdown and other embedded images)
            req.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
            byte[] bytes = new byte[8];
            Rand.NextBytes(bytes);
            req.HttpContext.Response.Headers.Add("ETag", Convert.ToBase64String(bytes));
            // Fetch Image
            var fact = await Fetch();
            // Return image
            var ms = new MemoryStream();
            await fact.SaveAsPngAsync(ms);
            ms.Position = 0;
            return new FileStreamResult(ms, "image/png");
        }
        public static async Task<Image> Fetch()
        {
            // Read RSS Feed to obtain the latest id
            var val = await FeedReader.ReadAsync("https://xkcd.com/atom.xml");
            // Clean url
            string pId = val.Items.First().Id.Substring(16).Replace("/", "");
            // Randomly pick a new id
            int rng = Rand.Next(1, int.Parse(pId) + 1);
            var webClient = new WebClient();
            // Load the xkcd website, and parse the html
            var parser = new HtmlParser().ParseDocument(webClient.DownloadString($"https://xkcd.com/{rng}"));
            // Grab the comic id
            var container = parser.GetElementById("comic");
            // In the child elements, find the src of the img tag
            var tParse = container.GetElementsByTagName("img")[0].GetAttribute("src");
            // Read the image into a memory stream
            var srcs = new MemoryStream(webClient.DownloadData("https://" + tParse.Substring(2)));
            // Ensure the source bitmap is PNG
            var img = await Image.LoadAsync(srcs);
            return img;
        }
    }
}
