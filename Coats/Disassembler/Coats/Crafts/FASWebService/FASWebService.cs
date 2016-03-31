namespace Coats.Crafts.FASWebService
{
    using System.CodeDom.Compiler;
    using System.ServiceModel;

    [GeneratedCode("System.ServiceModel", "4.0.0.0"), ServiceContract(Namespace="http://ns.fredhopper.com/XML/output/6.1.0", ConfigurationName="FASWebService.FASWebService")]
    public interface FASWebService
    {
        [OperationContract(Action="", ReplyAction="*"), XmlSerializerFormat(SupportFaults=true)]
        getAllResponse getAll(getAllRequest request);
    }
}

