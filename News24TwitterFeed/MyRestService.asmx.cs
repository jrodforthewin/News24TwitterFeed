using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.ChannelElements;
using Newtonsoft.Json;
using OAuth;

namespace News24TwitterFeed
{
    /// <summary>
    /// Summary description for MyRestService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class MyRestService : System.Web.Services.WebService
    {

        public class TwitterTokenData
        {
            public string token_type { get; set; }
            public string access_token { get; set; }
        }

        [WebMethod]
        public string GetNews24Feed()
        {
            var consumerKey = ConfigurationManager.AppSettings["consumer_key"].ToString();
            var consumerSecret = ConfigurationManager.AppSettings["consumer_secret"].ToString(); ;
            var tokenURL = "https://api.twitter.com/oauth2/token";
            var postBody = "grant_type=client_credentials";
            var authHeader = string.Format("Basic {0}",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(consumerKey) + ":" +
                                                              Uri.EscapeDataString((consumerSecret)))
                ));

            var twitterTokenData = GetAuthenticationResponse(tokenURL, authHeader, postBody);

            // Do the timeline
            var timelineFormat =
                "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&include_rts=1&exclude_replies=1&count=5";
            var handlename = "News24";
            var timelineUrl = string.Format(timelineFormat, handlename);
            HttpWebRequest timeLineRequest = (HttpWebRequest) WebRequest.Create(timelineUrl);
            var timelineHeaderFormat = "{0} {1}";
            timeLineRequest.Headers.Add("Authorization",
                string.Format(timelineHeaderFormat, twitterTokenData.token_type, twitterTokenData.access_token));
            timeLineRequest.Method = "Get";
            WebResponse timeLineResponse = timeLineRequest.GetResponse();
            var timeLineJson = string.Empty;
            using (timeLineResponse)
            {
                using (var reader = new StreamReader(timeLineResponse.GetResponseStream()))
                {
                    timeLineJson = reader.ReadToEnd();
                }
            }
            return timeLineJson;
            
        }

        private static TwitterTokenData GetAuthenticationResponse(string tokenUrl, string authHeader, string postBody)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(tokenUrl);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (Stream stream = request.GetRequestStream())
            {
                byte[] content = Encoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            request.Headers.Add("Accept-Encoding", "gzip");

            WebResponse response = request.GetResponse();
            // deserialize into an object
            TwitterTokenData twitterFeed;
            using (response)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objectText = reader.ReadToEnd();
                    twitterFeed = JsonConvert.DeserializeObject<TwitterTokenData>(objectText);
                }
            }
            return twitterFeed;
        }
    }
}
