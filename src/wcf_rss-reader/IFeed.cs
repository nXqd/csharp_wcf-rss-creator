using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;

namespace wcf_rss_reader {
    [ServiceContract]
    [ServiceKnownType(typeof (Atom10FeedFormatter))]
    [ServiceKnownType(typeof (Rss20FeedFormatter))]
    public interface IFeed {
        [OperationContract]
        [WebGet(UriTemplate = "GetFeeds?format={format}")]
        SyndicationFeedFormatter GetFeeds(string format);
    }
}