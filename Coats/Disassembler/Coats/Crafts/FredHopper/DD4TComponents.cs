namespace Coats.Crafts.FredHopper
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.FASWebService;
    using DD4T.ContentModel;
    using DD4T.Factories;
    using Repositories.Tridion;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web;
    using System.Xml;
    using System.Xml.Serialization;
    public class DD4TComponents : FredHopperInterface
    {
        private ILogger _logger;

        public DD4TComponents(ILogger logger)
        {
            this._logger = logger;
        }

        public static SiteMapNode FindSiteMapNodeByComponentId(string componentTcm)
        {
            IEnumerable<SiteMapNode> enumerable = SiteMap.RootNode.GetAllNodes().Cast<SiteMapNode>();
            foreach (SiteMapNode node2 in enumerable)
            {
                string str = node2["components"];
                if (!string.IsNullOrEmpty(str) && str.Contains(componentTcm))
                {
                    return node2;
                }
            }
            return null;
        }

        public List<Component> GetComponents(string fh_params)
        {
            return this.GetComponents(fh_params, false);
        }

        public List<Component> GetComponents(string fh_params, bool useWebConfigExtendedPropertyList)
        {
            return this.GetComponents(fh_params, useWebConfigExtendedPropertyList, false);
        }

        public List<Component> GetComponents(string fh_params, string extendedPropertyList)
        {
            return this.GetComponents(fh_params, extendedPropertyList, false);
        }

        public List<Component> GetComponents(string fh_params, bool useWebConfigExtendedPropertyList, bool setToActiveDiscussion)
        {
            return this.GetComponents(fh_params, useWebConfigExtendedPropertyList, setToActiveDiscussion, string.Empty);
        }

        public List<Component> GetComponents(string fh_params, string extendedPropertyList, bool setToActiveDiscussion)
        {
            return this.GetComponents(fh_params, extendedPropertyList, setToActiveDiscussion, null);
        }

        public List<Component> GetComponents(string fh_params, bool useWebConfigExtendedPropertyList, bool setToActiveDiscussion, string queryStringAppendage)
        {
            string extendedPropertyList = null;
            if (useWebConfigExtendedPropertyList)
            {
                extendedPropertyList = FredHopperExtendedProperties.Current.ToCommaDel();
            }
            return this.GetComponents(fh_params, extendedPropertyList, setToActiveDiscussion, queryStringAppendage);
        }

        public List<Component> GetComponents(string fh_params, string extendedPropertyList, bool setToActiveDiscussion, string queryStringAppendage)
        {
            this.Universe = base.CallFredHopper(fh_params);
            List<Component> list = this.ParseUniverse(extendedPropertyList);
            foreach (Component component in list)
            {
                if (setToActiveDiscussion)
                {
                    component.Schema.Title = component.Schema.Title + ".ActiveDiscussion";
                }
                SiteMapNode node = FindSiteMapNodeByComponentId(component.Id);
                if (node != null)
                {
                    if (!string.IsNullOrEmpty(queryStringAppendage))
                    {
                        component.Url = node.Url + (node.Url.Contains<char>('?') ? "&" : "?") + queryStringAppendage;
                    }
                    else
                    {
                        component.Url = node.Url;
                    }
                }
            }
            return list;
        }

        public List<Component> ParseUniverse(universe fhUniverse)
        {
            this.Universe = fhUniverse;
            return this.ParseUniverse(string.Empty);
        }

        protected List<Component> ParseUniverse(string extendedPropertyList)
        {
            this._logger.Debug("DD4TComponents > ParseUniverse >>>>>>>>");
            this._logger.DebugFormat("extendedPropertyList = {0}", new object[] { extendedPropertyList });
            List<Component> list = new List<Component>();
            XmlDocument document = null;
            try
            {
                int num = this.Universe.itemssection.items.Count<item>();
            }
            catch
            {
                return list;
            }
            this._logger.DebugFormat("Universe.itemssection.items.Count() = {0}", new object[] { this.Universe.itemssection.items.Count<item>() });


            var provider = new DD4T.Providers.WCFServices.TridionComponentProvider();
            foreach (item item in this.Universe.itemssection.items)
            {
                this._logger.InfoFormat("item = {0}", new object[] { item.id });

                // fetch component content XML from DD4T component factory
                string componentUri;
                string templateUri;
                if(DD4TComponents.TryGetTcmUrisFromSecondId(item.id, out componentUri, out templateUri))
                {
                    this._logger.DebugFormat("Split secondid {0} into component ID {1} and template ID {2}", new object[] { item.id, componentUri, templateUri });
                    string xml = provider.GetContent(componentUri);
                    this._logger.DebugFormat("Got component {0} XML from DD4T component factory: {1}", new object[] { item.id, xml });

                    if (!string.IsNullOrWhiteSpace(xml) && !string.IsNullOrEmpty(extendedPropertyList))
                    {
                        try
                        {
                            document = new XmlDocument();
                            document.LoadXml(xml);
                            Func<attribute, bool> predicate = null;
                            foreach (string propName in extendedPropertyList.Split(new char[] { ',' }))
                            {
                                StringBuilder builder = new StringBuilder();
                                builder.Append("<key><string>");
                                builder.Append(propName);
                                builder.Append("</string></key><value><Field XPath=\"");
                                builder.Append(propName);
                                builder.Append("\" FieldType=\"Text\"><Name>");
                                builder.Append(propName);
                                builder.Append("</Name><Values>");
                                if (predicate == null)
                                {
                                    predicate = a => a.name == propName;
                                }
                                attribute attribute2 = item.attribute.SingleOrDefault<attribute>(predicate);
                                if ((attribute2 != null) && (attribute2.value != null))
                                {
                                    foreach (value value2 in attribute2.value)
                                    {
                                        if ((value2 != null) && !string.IsNullOrEmpty(value2.Value))
                                        {
                                            builder.Append("<string>");
                                            builder.Append(value2.Value);
                                            builder.Append("</string>");
                                        }
                                    }
                                }
                                builder.Append("</Values><NumericValues/><DateTimeValues/><LinkedComponentValues/><EmbeddedValues/><Keywords/></Field></value>");
                                XmlNode newChild = document.CreateElement("item");
                                newChild.InnerXml = builder.ToString();
                                document.SelectSingleNode("/Component/Fields").AppendChild(newChild);
                            }
                            xml = document.OuterXml;
                            this._logger.DebugFormat("Final component Xml: {0}", new object[] { xml });
                        }
                        catch (Exception exception)
                        {
                            this._logger.Error("Error adding extended details", exception);
                        }
                    }
                    try
                    {
                        ComponentFactory factory = new ComponentFactory();
                        Component iComponentObject = (Component)factory.GetIComponentObject(xml);
                        list.Add(iComponentObject);
                    }
                    catch (Exception exception2)
                    {
                        this._logger.Error("Error getting DD4T component", exception2);
                    }
                }
            }
            return list;
        }

        public List<Component> ParseUniverse(universe fhUniverse, string extendedPropertyList)
        {
            this.Universe = fhUniverse;
            return this.ParseUniverse(string.Empty);
        }

        public universe Universe { get; set; }

        private static bool TryGetTcmUrisFromSecondId(string tcmuri, out string componentUri, out string templateUri)
        {
            bool result = false;
            componentUri = null;
            templateUri = null;
            if(!string.IsNullOrWhiteSpace(tcmuri) && tcmuri.StartsWith("tcm_"))
            {
                tcmuri = tcmuri.Replace("tcm_", "tcm:");
                string[] segments = tcmuri.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if(segments.Count() > 1)
                {
                    componentUri = segments[0].Replace("-16", string.Empty);
                    templateUri = segments[1];

                    result = !string.IsNullOrWhiteSpace(componentUri) &&
                             !string.IsNullOrWhiteSpace(templateUri) &&
                             componentUri.StartsWith("tcm:") &&
                             templateUri.StartsWith("tcm:");
                }
            }
            return result;
        }
    }
}

