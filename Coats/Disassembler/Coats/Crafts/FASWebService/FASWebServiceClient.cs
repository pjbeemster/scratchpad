namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    [DebuggerStepThrough, GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public class FASWebServiceClient : ClientBase<Coats.Crafts.FASWebService.FASWebService>, Coats.Crafts.FASWebService.FASWebService
    {
        public FASWebServiceClient()
        {
        }

        public FASWebServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public FASWebServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
        }

        public FASWebServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
        {
        }

        public FASWebServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        getAllResponse Coats.Crafts.FASWebService.FASWebService.getAll(getAllRequest request)
        {
            return base.Channel.getAll(request);
        }

        public page getAll(string fh_params)
        {
            getAllRequest request = new getAllRequest {
                fh_params = fh_params
            };
            return ((Coats.Crafts.FASWebService.FASWebService) this).getAll(request).page;
        }
    }
}

