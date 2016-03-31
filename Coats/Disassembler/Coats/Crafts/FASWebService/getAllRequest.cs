namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceModel;

    [DebuggerStepThrough, MessageContract(IsWrapped=false), GeneratedCode("System.ServiceModel", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Advanced)]
    public class getAllRequest
    {
        [MessageBodyMember(Namespace="http://ns.fredhopper.com/XML/output/6.1.0", Order=0)]
        public string fh_params;

        public getAllRequest()
        {
        }

        public getAllRequest(string fh_params)
        {
            this.fh_params = fh_params;
        }
    }
}

