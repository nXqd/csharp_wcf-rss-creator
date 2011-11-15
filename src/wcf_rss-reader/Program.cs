using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Xml;
using HtmlAgilityPack;

namespace wcf_rss_reader {
    public class FeedService : IFeed {
        private const string BaseUrl = "http://www.fit.hcmus.edu.vn/vn/";
        private static readonly List<string> Categories = new List<string> {
                                                                               "THÔNG TIN CHUNG",
                                                                               "THÔNG BÁO HỆ CQUI - CNTN",
                                                                               "THÔNG BÁO HỆ HCĐH",
                                                                               "THÔNG BÁO HỆ TẠI CHỨC",
                                                                               "THÔNG BÁO HỆ CAO ĐẲNG",
                                                                               "THÔNG BÁO HỆ SAU ĐẠI HỌC",
                                                                               "THÔNG TIN TUYỂN DỤNG",
                                                                               "THÔNG TIN HỘI THẢO - HỘI NGHỊ",
                                                                               "TIN TỨC THỜI SỰ"
                                                                           };

        #region IFeed Members

        public SyndicationFeedFormatter GetFeeds(string format) {
            var feed = new SyndicationFeed("Khoa công nghệ thông tin KHTN", "This is a test feed",
                                           new Uri("http://SomeURI"));
            feed.Authors.Add(new SyndicationPerson("nxqd.inbox@gmail.com - 0812090"));
            feed.Categories.Add(new SyndicationCategory("Feed"));
            feed.Description = new TextSyndicationContent("Tổng hợp các tin tức cháy bỏng tay từ khoa CNTT KHTN :>");
            feed.Items = GetItems();

            if (format == "rss")
                return new Rss20FeedFormatter(feed);
            return format == "atom" ? new Atom10FeedFormatter(feed) : null;
        }

        #endregion

        private IEnumerable<SyndicationItem> GetItems() {
            var doc = new HtmlWeb().Load("http://www.fit.hcmus.edu.vn/vn/Default.aspx?tabid=53");

            var feedItems = new List<SyndicationItem>();
            foreach (var des in doc.DocumentNode.SelectNodes("//div[@class='post_title']/a"))
                feedItems.Add(new SyndicationItem(des.InnerText, null, new Uri(BaseUrl + des.Attributes["href"].Value)));
            var counter = 0;
            foreach (var des in doc.DocumentNode.SelectNodes("//div[@class='day_month']")) {
                feedItems[counter].PublishDate = DateTime.Parse(des.InnerText, new CultureInfo("vi-vn", false)).Date;
                feedItems[counter].Categories.Add(new SyndicationCategory(Categories[counter/5]));
                counter++;
            }

            return feedItems;
        }
    }

    internal class Program {
        private static void Main(string[] args) {
            var address = new Uri("http://localhost:8000/FeedService/");
            var svcHost = new WebServiceHost(typeof (FeedService), address);
            try {
                svcHost.Open();
                Console.WriteLine("Service is running");
                Console.WriteLine("Open browser at http://localhost:8000/FeedService/GetFeeds?format={atom/rss}");
                Console.WriteLine("Press <ENTER> to quit...");
                Console.ReadLine();
                svcHost.Close();
            }
            catch (CommunicationException ce) {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                svcHost.Abort();
            }
        }
    }
}
