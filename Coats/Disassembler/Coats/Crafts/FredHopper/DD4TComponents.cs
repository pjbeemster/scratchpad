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

            var componentFactory = new ComponentFactory();
            var serializer = new Microsoft.Xml.Serialization.GeneratedAssembly.ComponentSerializer();

            foreach (item item in this.Universe.itemssection.items)
            {
                this._logger.DebugFormat("item = {0}", new object[] { item.id });

                // fetch component content XML from DD4T component factory
                string xml = null;
                IComponent component;
                if(componentFactory.TryGetComponent(item.id, out component))
                {
                    using(StringWriter sw = new StringWriter())
                    {
                        serializer.Serialize(sw, component);
                        xml = sw.ToString();
                    }
                    this._logger.DebugFormat("Got component {0} XML from DD4T component factory: {1}", item.id, xml);
                }
                else
                {
                    this._logger.DebugFormat("Failed to get component {0} from DD4T component factory", item.id);
                }

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
            return list;
        }

        public List<Component> ParseUniverse(universe fhUniverse, string extendedPropertyList)
        {
            this.Universe = fhUniverse;
            return this.ParseUniverse(string.Empty);
        }

        public universe Universe { get; set; }
    }
}

