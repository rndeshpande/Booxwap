namespace Booxwap.Social.AmazonHelpers
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    public class AmazonSigningEndpointBehavior : IEndpointBehavior
    {
        private readonly string accessKeyId = "";
        private readonly string _secretKey = "";
        public string FileGuid = "";

        public AmazonSigningEndpointBehavior(string accessKeyId, string secretKey, string fileGuid)
        {
            this.accessKeyId = accessKeyId;
            this._secretKey = secretKey;
            this.FileGuid = fileGuid;
        }

        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new AmazonSigningMessageInspector(accessKeyId, _secretKey, FileGuid));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            return;
        }

        public void Validate(ServiceEndpoint serviceEndpoint)
        {
            return;
        }

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
        {
            return;
        }
    }
}