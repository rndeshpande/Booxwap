using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;

namespace Booxwap.Social.AmazonHelpers
{
    public class AmazonSigningMessageInspector : IClientMessageInspector
    {
        private string accessKeyId = "xxxx";
        private string secretKey = "xxxx";
        public string FileGUID = "";

        public AmazonSigningMessageInspector(string accessKeyId, string secretKey, string FileGUID)
        {
            this.accessKeyId = accessKeyId;
            this.secretKey = secretKey;
            this.FileGUID = FileGUID;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // prepare the data to sign
            string operation = Regex.Match(request.Headers.Action, "[^/]+$").ToString();
            DateTime now = DateTime.UtcNow;
            string timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string signMe = operation + timestamp;
            byte[] bytesToSign = Encoding.UTF8.GetBytes(signMe);

            // sign the data
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            HMAC hmacSha256 = new HMACSHA256(secretKeyBytes);
            byte[] hashBytes = hmacSha256.ComputeHash(bytesToSign);
            string signature = Convert.ToBase64String(hashBytes);

            // add the signature information to the request headers
            request.Headers.Add(new AmazonHeader("AWSAccessKeyId", accessKeyId));
            request.Headers.Add(new AmazonHeader("Timestamp", timestamp));
            request.Headers.Add(new AmazonHeader("Signature", signature));

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            String strResponse = string.Empty;
            strResponse = reply.ToString().Replace("{", "").Replace("xmlns=\"http://webservices.amazon.com/AWSECommerceService/2011-08-01\"", "");
            String strFileName = ConfigurationManager.AppSettings["ExceptionLog"].ToString() + FileGUID + ".xml";

            TextWriter tw = new StreamWriter(strFileName);
            tw.Write(strResponse);
            tw.Close();
        }
    }
}