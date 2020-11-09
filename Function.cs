using System;
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

namespace random_xkcd
{
    public static class Function
    {

        [FunctionName("xkcd")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            // Read RSS Feed to obtain the latest id
            var val = await FeedReader.ReadAsync("https://xkcd.com/atom.xml");
            // Clean url
            string pId = val.Items.First().Id.Substring(16).Replace("/","");
            Random rand = new Random();
            // Randomly pick a new id
            int rng = rand.Next(1, int.Parse(pId) + 1);
            var webClient = new WebClient();
            // Load the xkcd website, and parse the html
            var parser = new HtmlParser().ParseDocument(webClient.DownloadString($"https://xkcd.com/{rng}"));
            // Grab the comic id
            var container = parser.GetElementById("comic");
            // In the child elements, find the src of the img tag
            var tParse = container.GetElementsByTagName("img")[0].GetAttribute("src");
            // Read the image into a memory stream
            var ms = new MemoryStream(webClient.DownloadData("https://" + tParse.Substring(2)));
            // Disable caching, (most sites like Github cache markdown and other embedded images)
            req.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
            req.HttpContext.Response.Headers.Add("ETag", pId);
            // Return image
            return new FileStreamResult(ms, "image/png");
        }
    }
}
