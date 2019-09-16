using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using CodeFrames;
using Newtonsoft.Json;

namespace CodeFramesAPI
{
    public class DanbooruImageGetter : IFrameValueGetter
    {
        private Queue<string> ImageUrls { get; }

        public DanbooruImageGetter()
        {
            ImageUrls = new Queue<string>();
            Reset();
        }

        public string GetNext()
        {
            return ImageUrls.Dequeue();
        }

        public void Reset()
        {
            ImageUrls.Clear();
            string resp = Get("https://safebooru.donmai.us/posts.json?random=true&limit=60&tags=filesize:10kb..20kb");
            //string resp = Get("https://safebooru.donmai.us/posts.json?limit=60&tags=filesize:200kb..400kb");
            List<DanbooruImage> images = JsonConvert.DeserializeObject<List<DanbooruImage>>(resp);
            images = FilterBadLinks(images);
            images.ForEach(i => ImageUrls.Enqueue(i.FileUrl));
        }

        private List<DanbooruImage> FilterBadLinks(List<DanbooruImage> images)
        {
            return images.Where(i => !UrlIsEmpty(i) && UrlAllowsLinking(i)).ToList();
        }

        private bool UrlIsEmpty(DanbooruImage image)
        {
            return string.IsNullOrEmpty(image.FileUrl);
        }

        private bool UrlAllowsLinking(DanbooruImage image)
        {
            return false == image.FileUrl.Contains("safebooru.donmai");
        }

        private string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public class DanbooruImage
        {
            [JsonProperty("preview_file_url")]
            public string PreviewFileUrl { get; set; }

            [JsonProperty("file_url")]
            public string FileUrl { get; set; }

            [JsonProperty("large_file_url")]
            public string LargeFileUrl { get; set; }
        }
    }
}