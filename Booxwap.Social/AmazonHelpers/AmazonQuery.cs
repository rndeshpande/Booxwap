using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Xml;

namespace Booxwap.Social.AmazonHelpers
{
    using AmazonWS;
    
    public class AmazonQuery
    {
        // Note: protocol must be https for signed SOAP requests.
        //private const String DESTINATION = "https://ecs.amazonaws.com/onca/soap?Service=AWSECommerceService";

        // Set your AWS Access Key ID and AWS Secret Key here.
        // You can obtain them at:
        // http://aws-portal.amazon.com/gp/aws/developer/account/index.html?action=access-key
        private const String MyAwsId = "xxxx";

        private const String MyAwsSecret = "xxxx";
        public static int PageNumber = 1;

        private const string MyAwsAccessKeyId = "xxxx";
        private const string MyAwsSecretKey = "xxxx";
        private const string Destination = "ecs.amazonaws.com";

        private const string Namespace = "http://webservices.amazon.com/AWSECommerceService/2009-03-31";

        public DataTable Search(string searchType, string searchText)
        {
            string returnValue = string.Empty;
            DataTable dtBooks = new DataTable();
            dtBooks.Columns.Add("Title", typeof(string));
            dtBooks.Columns.Add("Author", typeof(string));
            dtBooks.Columns.Add("AmazonURL", typeof(string));
            dtBooks.Columns.Add("BookName", typeof(string));
            dtBooks.Columns.Add("Type", typeof(string));
            dtBooks.Columns.Add("ASIN", typeof(string));

            // create a WCF Amazon ECS client
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            AWSECommerceServicePortTypeClient client = new AWSECommerceServicePortTypeClient(
                binding,
                new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));

            // add authentication to the ECS client
            string strFileGuid = Guid.NewGuid().ToString();
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(MyAwsId, MyAwsSecret, strFileGuid));

            // prepare an ItemSearch request
            ItemSearchRequest request = new ItemSearchRequest();
            if (searchType == "Title")
            {
                request.SearchIndex = "Books";
                request.Title = searchText;
                request.Sort = "salesrank";
            }
            else
                if (searchType == "Author")
                {
                    request.SearchIndex = "Books";
                    request.Author = searchText;
                    request.Sort = "salesrank";
                }
            request.ResponseGroup = new string[] { "Small" };

            request.ItemPage = PageNumber.ToString();

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.AssociateTag = "booxwap-20";
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AWSAccessKeyId = MyAwsId;

            ItemSearchResponse searchResponse = new ItemSearchResponse();

            searchResponse = client.ItemSearch(itemSearch);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(ConfigurationManager.AppSettings["ExceptionLog"].ToString() + strFileGuid + ".xml");

            File.Delete(ConfigurationManager.AppSettings["ExceptionLog"].ToString() + strFileGuid + ".xml");

            XmlNamespaceManager mgr = new XmlNamespaceManager(xmlDoc.NameTable);
            mgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");

            XmlNodeList nodeList = xmlDoc.SelectNodes("/s:Envelope/s:Body/ItemSearchResponse/Items/Item", mgr);
            string strBookList = string.Empty;
            dtBooks.Clear();
            string strBookName = string.Empty;
            string strAuthor = string.Empty;
            string strAmazonURL = string.Empty;
            string strTitle = string.Empty;
            string strBookType = string.Empty;
            string strASIN = string.Empty;

            foreach (XmlNode node in nodeList)
            {
                DataRow dr = dtBooks.NewRow();
                strTitle = "";
                strAuthor = "";
                strAmazonURL = "";
                strBookType = "";
                strBookName = "";
                strASIN = "";

                if (node.SelectSingleNode("ItemAttributes/Title") != null)
                {
                    strBookName = node.SelectSingleNode("ItemAttributes/Title").InnerText.ToString();
                    strTitle = strBookName;
                }
                if (node.SelectSingleNode("ItemAttributes/Author") != null)
                {
                    strAuthor = node.SelectSingleNode("ItemAttributes/Author").InnerText.ToString();
                    strTitle += " (" + strAuthor + ") ";
                }
                if (node.SelectSingleNode("DetailPageURL") != null)
                {
                    strAmazonURL = node.SelectSingleNode("DetailPageURL").InnerText.ToString();
                }
                if (node.SelectSingleNode("ASIN") != null)
                {
                    strASIN = node.SelectSingleNode("ASIN").InnerText.ToString();
                }
                if (node.SelectSingleNode("ItemAttributes/ProductGroup") != null)
                    if (node.SelectSingleNode("ItemAttributes/ProductGroup").InnerText.ToString().ToUpper() == "EBOOKS")
                    {
                        strTitle += " <i>eBook</i>";
                        strBookType = "ebook";
                    }
                    else
                        strBookType = "regular";

                dr["Title"] = strTitle;
                dr["Author"] = strAuthor;
                dr["AmazonURL"] = strAmazonURL;
                dr["BookName"] = strBookName;
                dr["Type"] = strBookType;
                dr["ASIN"] = strASIN;
                dtBooks.Rows.Add(dr);
            }

            return dtBooks;
        }

        public DataTable LookupItem(string strBookAsin)
        {
            string returnValue = string.Empty;
            DataTable dtBooks = new DataTable();
            dtBooks.Columns.Add("Title", typeof(string));
            dtBooks.Columns.Add("Author", typeof(string));
            dtBooks.Columns.Add("AmazonURL", typeof(string));
            dtBooks.Columns.Add("Description", typeof(string));
            dtBooks.Columns.Add("AmazonReview", typeof(string));
            dtBooks.Columns.Add("Picture", typeof(string));
            dtBooks.Columns.Add("PictureHeight", typeof(string));
            dtBooks.Columns.Add("PictureWidth", typeof(string));

            // create a WCF Amazon ECS client
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            var client = new AWSECommerceServicePortTypeClient(
                binding,
                new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));

            // add authentication to the ECS client
            var strFileGuid = Guid.NewGuid().ToString();
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(MyAwsId, MyAwsSecret, strFileGuid));

            string[] strBookAsinarr = { strBookAsin };
            var request = new ItemLookupRequest
            {
                IdType = ItemLookupRequestIdType.ASIN,
                ItemId = strBookAsinarr,
                ResponseGroup = new string[] { "Large" }
            };

            ItemLookup itemLookup = new ItemLookup
            {
                AssociateTag = "booxwap-20",
                Request = new ItemLookupRequest[] { request },
                AWSAccessKeyId = MyAwsId
            };

            var searchResponse = client.ItemLookup(itemLookup);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(ConfigurationManager.AppSettings["ExceptionLog"].ToString() + strFileGuid + ".xml");

            File.Delete(ConfigurationManager.AppSettings["ExceptionLog"].ToString() + strFileGuid + ".xml");

            var mgr = new XmlNamespaceManager(xmlDoc.NameTable);
            mgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");

            XmlNodeList nodeList = xmlDoc.SelectNodes("/s:Envelope/s:Body/ItemLookupResponse/Items/Item", mgr);
            string strBookList = string.Empty;
            dtBooks.Clear();
            string strDescription;
            string strAmazonReview;
            string strAuthor;
            string strAmazonURL;
            string strTitle;
            string strPicture;
            string strPictureHeight;
            string strPictureWidth;

            if (nodeList != null)
                foreach (XmlNode node in nodeList)
                {
                    DataRow dr = dtBooks.NewRow();
                    strTitle = "";
                    strAuthor = "";
                    strAmazonURL = "";
                    strPicture = "";
                    strDescription = "";
                    strAmazonReview = "";
                    strPictureHeight = "";
                    strPictureWidth = "";

                    if (node.SelectSingleNode("ItemAttributes/Title") != null)
                    {
                        var selectSingleNode = node.SelectSingleNode("ItemAttributes/Title");
                        if (selectSingleNode != null)
                            strTitle = selectSingleNode.InnerText.ToString();
                    }
                    if (node.SelectSingleNode("ItemAttributes/Author") != null)
                    {
                        var selectSingleNode = node.SelectSingleNode("ItemAttributes/Author");
                        if (selectSingleNode != null)
                            strAuthor = selectSingleNode.InnerText.ToString();
                    }
                    if (node.SelectSingleNode("DetailPageURL") != null)
                    {
                        var selectSingleNode = node.SelectSingleNode("DetailPageURL");
                        if (selectSingleNode != null)
                            strAmazonURL = selectSingleNode.InnerText.ToString();
                    }
                    if (node.SelectSingleNode("MediumImage") != null)
                    {
                        var selectSingleNode = node.SelectSingleNode("MediumImage/URL");
                        if (selectSingleNode != null)
                            strPicture = selectSingleNode.InnerText.ToString();
                        var singleNode = node.SelectSingleNode("MediumImage/Height");
                        if (singleNode != null)
                            strPictureHeight = singleNode.InnerText.ToString();
                        var xmlNode = node.SelectSingleNode("MediumImage/Width");
                        if (xmlNode != null)
                            strPictureWidth = xmlNode.InnerText.ToString();
                    }
                    XmlNodeList nodeReviews = node.SelectNodes("EditorialReviews/EditorialReview");
                    foreach (XmlNode nodeReview in nodeReviews)
                    {
                        var selectSingleNode = nodeReview.SelectSingleNode("Source");
                        if (selectSingleNode != null && selectSingleNode.InnerText.ToString() == "Product Description")
                        {
                            var singleNode = nodeReview.SelectSingleNode("Content");
                            if (singleNode != null)
                                strDescription = singleNode.InnerText.ToString();
                        }
                        else
                        {
                            var xmlNode = nodeReview.SelectSingleNode("Source");
                            if (xmlNode != null && xmlNode.InnerText.ToString() == "Amazon.com Review")
                            {
                                var singleNode = nodeReview.SelectSingleNode("Content");
                                if (singleNode != null)
                                    strAmazonReview = singleNode.InnerText.ToString();
                            }
                        }
                    }

                    dr["Title"] = strTitle;
                    dr["Author"] = strAuthor;
                    dr["AmazonURL"] = strAmazonURL;
                    dr["Description"] = strDescription;
                    dr["AmazonReview"] = strAmazonReview;
                    dr["Picture"] = strPicture;
                    dr["PictureHeight"] = strPictureHeight;
                    dr["PictureWidth"] = strPictureWidth;
                    dtBooks.Rows.Add(dr);
                }

            return dtBooks;
        }

        public DataTable LookupItemNew(string bookAsin)
        {
            SignedRequestHelper helper = new SignedRequestHelper(MyAwsAccessKeyId, MyAwsSecretKey, Destination);

            /*
             * The helper supports two forms of requests - dictionary form and query string form.
             */

            /*
             * Here is an ItemLookup example where the request is stored as a dictionary.
             */
            var r1 = new Dictionary<string, String>();
            r1["AssociateTag"] = "booxwap-20";
            r1["Service"] = "AWSECommerceService";
            r1["Version"] = "2009-03-31";
            r1["Operation"] = "ItemLookup";
            r1["ItemId"] = bookAsin;
            r1["ResponseGroup"] = "Large";

            var requestUrl = helper.Sign(r1);
            return FetchTitle(requestUrl);
        }

        private static DataTable FetchTitle(string url)
        {
            try
            {
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();
                XmlDocument doc = new XmlDocument();
                doc.Load(response.GetResponseStream());

                XmlNodeList errorMessageNodes = doc.GetElementsByTagName("Message", Namespace);
                if (errorMessageNodes != null && errorMessageNodes.Count > 0)
                {
                    String message = errorMessageNodes.Item(0).InnerText;
                    var errMessage = "Error: " + message + " (but signature worked)";
                    return null;
                }

                //XmlNode titleNode = doc.GetElementsByTagName("Title", NAMESPACE).Item(0);
                //string title = titleNode.InnerText;
                //return title;
                var dtBooks = new DataTable();
                var mgr = new XmlNamespaceManager(doc.NameTable);
                mgr.AddNamespace("s", Namespace);
                XmlNodeList nodeList = doc.SelectNodes("/s:Envelope/s:Body/ItemLookupResponse/Items/Item", mgr);
                string strBookList = string.Empty;
                dtBooks.Clear();

                foreach (XmlNode node in nodeList)
                {
                    DataRow dr = dtBooks.NewRow();
                    var strTitle = "";
                    var strAuthor = "";
                    var strAmazonUrl = "";
                    var strPicture = "";
                    var strDescription = "";
                    var strAmazonReview = "";
                    var strPictureHeight = "";
                    var strPictureWidth = "";

                    if (node.SelectSingleNode("ItemAttributes/Title") != null)
                    {
                        strTitle = node.SelectSingleNode("ItemAttributes/Title").InnerText.ToString();
                    }
                    if (node.SelectSingleNode("ItemAttributes/Author") != null)
                    {
                        strAuthor = node.SelectSingleNode("ItemAttributes/Author").InnerText.ToString();
                    }
                    if (node.SelectSingleNode("DetailPageURL") != null)
                    {
                        strAmazonUrl = node.SelectSingleNode("DetailPageURL").InnerText.ToString();
                    }
                    if (node.SelectSingleNode("MediumImage") != null)
                    {
                        strPicture = node.SelectSingleNode("MediumImage/URL").InnerText.ToString();
                        strPictureHeight = node.SelectSingleNode("MediumImage/Height").InnerText.ToString();
                        strPictureWidth = node.SelectSingleNode("MediumImage/Width").InnerText.ToString();
                    }
                    XmlNodeList nodeReviews = node.SelectNodes("EditorialReviews/EditorialReview");
                    foreach (XmlNode nodeReview in nodeReviews)
                    {
                        if (nodeReview.SelectSingleNode("Source").InnerText.ToString() == "Product Description")
                        {
                            strDescription = nodeReview.SelectSingleNode("Content").InnerText.ToString();
                        }
                        else
                            if (nodeReview.SelectSingleNode("Source").InnerText.ToString() == "Amazon.com Review")
                            {
                                strAmazonReview = nodeReview.SelectSingleNode("Content").InnerText.ToString();
                            }
                    }

                    dr["Title"] = strTitle;
                    dr["Author"] = strAuthor;
                    dr["AmazonURL"] = strAmazonUrl;
                    dr["Description"] = strDescription;
                    dr["AmazonReview"] = strAmazonReview;
                    dr["Picture"] = strPicture;
                    dr["PictureHeight"] = strPictureHeight;
                    dr["PictureWidth"] = strPictureWidth;
                    dtBooks.Rows.Add(dr);
                }

                return dtBooks;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Caught Exception: " + e.Message);
                System.Console.WriteLine("Stack Trace: " + e.StackTrace);
            }

            return null;
        }
    }
}