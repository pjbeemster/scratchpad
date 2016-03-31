namespace Coats.Crafts.FASWebService
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceModel;

    [EditorBrowsable(EditorBrowsableState.Advanced), GeneratedCode("System.ServiceModel", "4.0.0.0"), DebuggerStepThrough, MessageContract(IsWrapped=false)]
    public class getAllResponse
    {
        [MessageBodyMember(Namespace="http://ns.fredhopper.com/XML/output/6.1.0", Order=0)]
        public Coats.Crafts.FASWebService.page page;

        public getAllResponse()
        {
        }

        public getAllResponse(Coats.Crafts.FASWebService.page page)
        {
            this.page = page;
        }
    }
}

