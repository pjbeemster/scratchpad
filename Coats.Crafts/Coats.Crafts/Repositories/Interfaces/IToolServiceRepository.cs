using System.Collections.Generic;
using Coats.Crafts.Models;

namespace Coats.Crafts.Repositories.Interfaces
{
    public interface IToolServiceRepository
    {
        IList<ToolService> GetToolsService(string identifier, string groupname);
        IList<ToolService> GetToolsService(string identifier, string groupname, string[] accessKeys);
        IList<ToolService> GetToolsServiceByRelated(IList<ToolService> listTool, string identifier);
        string GetToolUrl(string toolserviceName);
        string GetServiceUrl(string toolserviceName);
    }
}
