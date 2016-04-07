namespace Coats.Crafts.FredHopper
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.FASWebService;
    using DD4T.ContentModel;
    using DD4T.Factories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Xml;

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
                    if (!string.IsNullOrWhiteSpace(xml) && !string.IsNullOrEmpty(extendedPropertyList))
                    {
                        try
                        {
                            document = new XmlDocument();
                            document.LoadXml(xml);

                            // --------------------------------------------------------
                            // An example XML snippet of what we are trying to achieve
                            // --------------------------------------------------------
                            // <item>
                            //   <key>
                            //     <string>[propName]</string>
                            //   </key>
                            //   <value>
                            //     <Field XPath="[propName]???" FieldType="Text">
                            //       <Name>[propName]</Name>
                            //       <Values>
                            //         <string>abc</string>
                            //         <string>def</string>
                            //         <string>ghi</string>
                            //       </Values>
                            //       <NumericValues/>
                            //       <DateTimeValues/>
                            //       <LinkedComponentValues/>
                            //       <EmbeddedValues/>
                            //       <Keywords/>
                            //     </Field>
                            //   </value>
                            // </item>
                            // --------------------------------------------------------

                            // Loop through the extended ptrpoerty attribute list
                            foreach (string propName in extendedPropertyList.Split(','))
                            {
                                // Start to build the XML snippet (I'm cheating and using a StringBuilder!)
                                StringBuilder sb = new StringBuilder();
                                sb.Append("<key><string>");
                                sb.Append(propName);
                                sb.Append("</string></key><value><Field XPath=\"");
                                sb.Append(propName);
                                sb.Append("\" FieldType=\"Text\"><Name>");
                                sb.Append(propName);
                                sb.Append("</Name><Values>");

                                // Try and find the actual attribute in the FredHopper item
                                var attr = item.attribute.SingleOrDefault(a => a.name == propName);
                                if (attr != null)
                                {
                                    if (attr.value != null)
                                    {
                                        // Could be an array of values, so loop through
                                        foreach (var valueItem in attr.value)
                                        {
                                            // Belt and braces check to make sure this object isnt null
                                            if (valueItem != null)
                                            {
                                                // Double-check value
                                                if (!String.IsNullOrEmpty(valueItem.Value))
                                                {
                                                    sb.Append("<string>");
                                                    sb.Append(valueItem.Value);
                                                    sb.Append("</string>");
                                                }
                                            }
                                        }
                                    }
                                }

                                // Finally, close off the XML snippet
                                sb.Append("</Values><NumericValues/><DateTimeValues/><LinkedComponentValues/><EmbeddedValues/><Keywords/></Field></value>");

                                // Create a new XML Node to import the XML snippet
                                XmlNode xmlItem = document.CreateElement("item");
                                xmlItem.InnerXml = sb.ToString();

                                // Navigate to the relevant instertion point
                                XmlNode fields = document.SelectSingleNode("/Component/Fields");

                                // Now insert the new node into the overall DD4T component XML
                                fields.AppendChild(xmlItem);
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
                    componentUri = segments[0];
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

