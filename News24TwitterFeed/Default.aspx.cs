using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OAuth;
using System.Web.Script.Serialization;

namespace News24TwitterFeed
{
    public partial class _Default : Page
    {

        public class TwitterFeed
        {
            public string Text { get; set; }
            public int RetweetCount { get; set; }
            public int FavouriteCount { get; set; }
        }

        public List<TwitterFeed> lTwitterFeeds = new List<TwitterFeed>();
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnGetTwitterFeed_OnClick(object sender, EventArgs e)
        {
            try
            {
                lTwitterFeeds = new List<TwitterFeed>();
                using (MyRestService service = new MyRestService())
                {
                    string json = service.GetNews24Feed();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    dynamic items = serializer.Deserialize<object>(json);

                    foreach (var item in items)
                    {
                        TwitterFeed tw = new TwitterFeed();
                        foreach (KeyValuePair<string, object> kvp in item)
                        {
                            switch (kvp.Key)
                            {
                                case "text":
                                    tw.Text = Linkify(kvp.Value.ToString());
                                    break;
                                case "retweet_count":
                                    tw.RetweetCount = Convert.ToInt32(kvp.Value.ToString());
                                    break;
                                case "favorite_count":
                                    tw.FavouriteCount = Convert.ToInt32(kvp.Value.ToString());
                                    break;

                            }

                        }
                        lTwitterFeeds.Add(tw);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string Linkify(string searchText)
        {
            Regex regx = new Regex(@"\b(((\S+)?)(@|mailto\:|(news|(ht|f)tp(s?))\://)\S+)\b", RegexOptions.IgnoreCase);
            searchText = searchText.Replace("&nbsp;", " ");
            MatchCollection matches = regx.Matches(searchText);

            foreach (Match match in matches)
            {
                if (match.Value.StartsWith("http"))
                { // if it starts with anything else then dont linkify -- may already be linked!
                    searchText = searchText.Replace(match.Value, "<a href='" + match.Value + "' target='_blank'>" + Check24Url(match.Value)+ "</a>");
                }
            }

            return searchText;
        }

        private string Check24Url(string value )
        {
            switch (value)
            {
                case "https://t.co/By0KBr5HQV":
                    return "News24.com";
                case "https://t.co/9duT1K3Wpt":
                    return "Fin24.com";
                case "https://t.co/pdTxfqQbDI":
                    return "Sports24.co.za";
                case "https://t.co/abxxUzfiyn":
                    return "Channel24.com";
            }
            return value;
        }
    }
}