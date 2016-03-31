using System;
using System.Collections.Generic;
using System.Linq;
using Coats.Crafts.FASWebService;
using System.Text;
using DD4T.ContentModel;
using System.Xml;
using DD4T.Factories;
using System.Web;
using System.Configuration;
using Coats.Crafts.Extensions;
using Coats.Crafts.Configuration;

using Castle.Core.Logging;
using Castle.Windsor;

namespace Coats.Crafts.FredHopper
{
    /// <summary>
    /// Utility class to get DD4T components out of FredHopper.
    /// </summary>
    public class DD4TComponents : FredHopperInterface
    {
        private ILogger _logger;

        public DD4TComponents(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Holds the complete, selected FredHopper response
        /// </summary>
        public universe Universe { get; set; }

        /// <summary>
        /// Retrieves all the DD4T components from FredHopper for the specified query path
        /// </summary>
        /// <param name="fh_params">
        /// Usually supplied by creating the required query object, then performing a toString() on it.
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> GetComponents(string fh_params)
        {
            return GetComponents(fh_params, false);
        }

        /// <summary>
        /// Retrieves all the DD4T components from FredHopper for the specified query path
        /// </summary>
        /// <param name="fh_params">
        /// Usually supplied by creating the required query object, then performing a toString() on it.
        /// </param>
        /// <param name="useWebConfigExtendedPropertyList">
        /// Denotes whether to use the extended property list from the web configuration settings.
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> GetComponents(string fh_params, bool useWebConfigExtendedPropertyList)
        {
            return GetComponents(fh_params, useWebConfigExtendedPropertyList, false);
        }

        /// <summary>
        /// Retrieves all the DD4T components from FredHopper for the specified query path
        /// </summary>
        /// <param name="fh_params">
        /// Usually supplied by creating the required query object, then performing a toString() on it.
        /// </param>
        /// <param name="useWebConfigExtendedPropertyList">
        /// Denotes whether to use the extended property list from the web configuration settings.
        /// </param>
        /// <param name="setToActiveDiscussion">
        /// Boolean switch to decide whether to set an attribute within the DD4T component to flag an active discussion
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> GetComponents(string fh_params, bool useWebConfigExtendedPropertyList, bool setToActiveDiscussion)
        {
            return GetComponents(fh_params, useWebConfigExtendedPropertyList, setToActiveDiscussion, string.Empty);
        }

        /// <summary>
        /// Retrieves all the DD4T components from FredHopper for the specified query path
        /// </summary>
        /// <param name="fh_params">
        /// Usually supplied by creating the required query object, then performing a toString() on it.
        /// </param>
        /// <param name="useWebConfigExtendedPropertyList">
        /// Denotes whether to use the extended property list from the web configuration settings.
        /// </param>
        /// <param name="setToActiveDiscussion">
        /// Boolean switch to decide whether to set an attribute within the DD4T component to flag an active discussion
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> GetComponents(string fh_params, bool useWebConfigExtendedPropertyList, bool setToActiveDiscussion, string queryStringAppendage)
        {
            string extendedPropertyList = null;
            if (useWebConfigExtendedPropertyList)
            {
                extendedPropertyList = FredHopperExtendedProperties.Current.ToCommaDel();
            }
            return GetComponents(fh_params, extendedPropertyList, setToActiveDiscussion, queryStringAppendage);
        }

        /// <summary>
        /// Retrieves all the DD4T components from FredHopper for the specified query path
        /// </summary>
        /// <param name="fh_params">
        /// Usually supplied by creating the required query object, then performing a toString() on it.
        /// </param>
        /// <param name="extendedPropertyList">
        /// A comma separating list of FredHopper attributes to be inserted into the DD4T Component XML
        /// before deserialisation.
        /// e.g. "commentcount,rating,commenturl"
        /// Can be null.
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> GetComponents(string fh_params, string extendedPropertyList) 
        {
            //Universe = CallFredHopper(fh_params);
            //return ParseUniverse(extendedPropertyList);
            return GetComponents(fh_params, extendedPropertyList, false);
        }

        /// <summary>
        /// Retrieves all the DD4T components from FredHopper for the specified query path.
        /// The main difference is the setToActiveDiscussion param. This will append ".ActiveDiscussion"
        /// to the Schema.Title field for each component returned.
        /// </summary>
        /// <param name="fh_params">
        /// Usually supplied by creating the required query object, then performing a toString() on it.
        /// </param>
        /// <param name="extendedPropertyList">
        /// A comma separating list of FredHopper attributes to be inserted into the DD4T Component XML
        /// before deserialisation.
        /// e.g. "commentcount,rating,commenturl"
        /// Can be null.
        /// </param>
        /// <param name="setToActiveDiscussion">
        /// Boolean switch to decide whether to set an attribute within the DD4T component to flag an active discussion
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> GetComponents(string fh_params, string extendedPropertyList, bool setToActiveDiscussion)
        {
            return GetComponents(fh_params, extendedPropertyList, setToActiveDiscussion, null);
        }

        /// <summary>
        /// Retrieves all the DD4T components from FredHopper for the specified query path.
        /// The main difference is the setToActiveDiscussion param. This will append ".ActiveDiscussion"
        /// to the Schema.Title field for each component returned.
        /// </summary>
        /// <param name="fh_params">
        /// Usually supplied by creating the required query object, then performing a toString() on it.
        /// </param>
        /// <param name="extendedPropertyList">
        /// A comma separating list of FredHopper attributes to be inserted into the DD4T Component XML
        /// before deserialisation.
        /// e.g. "commentcount,rating,commenturl"
        /// Can be null.
        /// </param>
        /// <param name="setToActiveDiscussion">
        /// Boolean switch to decide whether to set an attribute within the DD4T component to flag an active discussion
        /// </param>
        /// <param name="queryStringAppendage">
        /// Chance to append a query string to the end of the Url
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> GetComponents(string fh_params, string extendedPropertyList, bool setToActiveDiscussion, string queryStringAppendage)
        {
            Universe = CallFredHopper(fh_params);
            List<Component> components = ParseUniverse(extendedPropertyList);
            foreach (Component comp in components)
            {
                if (setToActiveDiscussion)
                {
                    comp.Schema.Title = string.Concat(comp.Schema.Title, ".ActiveDiscussion");
                }
                //var tsp = (Coats.Crafts.Sitemap.TridionSiteMapProvider)SiteMap.Provider;
                //var node = tsp.FindSiteMapNodeByComponentId("tcm:70-19778");
                //var node = tsp.FindSiteMapNodeByComponentId(comp.Id);

                var node = FindSiteMapNodeByComponentId(comp.Id);
                //var node = Sitemap.TridionSiteMapProvider.FindSiteMapNodeByComponentId(comp.Id);
                if (node != null)
                {
                    if (!string.IsNullOrEmpty(queryStringAppendage))
                    {
                        comp.Url = node.Url +
                        (node.Url.Contains('?') ? "&" : "?") +
                        queryStringAppendage;
                    }
                    else
                    {
                        comp.Url = node.Url;
                    }
                }
            }

            return components;
        }


        public static SiteMapNode FindSiteMapNodeByComponentId(string componentTcm)
        {
            IEnumerable<SiteMapNode> nodes = SiteMap.RootNode
                                                .GetAllNodes()
                                                .Cast<SiteMapNode>();

            SiteMapNode node = null;
            foreach (SiteMapNode n in nodes)
            {
                string components = n["components"];
                if (!String.IsNullOrEmpty(components))
                {
                    if (components.Contains(componentTcm))
                    {
                        node = n;
                        break;
                    }
                }
            }

            //SiteMapNode node = nodes.FirstOrDefault(n => n["components"].Contains(componentTcm));

            return node;
        }

        /// <summary>
        /// Parses the specified FredHopper Universe into a DD4T Component list
        /// </summary>
        /// <param name="fhUniverse">
        /// A valid FredHopper Universe, usually obtained from a response from a call to FredHopper.
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> ParseUniverse(universe fhUniverse)
        {
            Universe = fhUniverse;
            return ParseUniverse(string.Empty);
        }

        /// <summary>
        /// Parses the specified FredHopper Universe into a DD4T Component list
        /// </summary>
        /// <param name="fhUniverse">
        /// A valid FredHopper Universe, usually obtained from a response from a call to FredHopper.
        /// </param>
        /// <param name="extendedPropertyList">
        /// A comma separating list of FredHopper attributes to be inserted into the DD4T Component XML
        /// before deserialisation.
        /// e.g. "commentcount,rating,commenturl".
        /// Can be null.
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        public List<Component> ParseUniverse(universe fhUniverse, string extendedPropertyList)
        {
            Universe = fhUniverse;
            return ParseUniverse(string.Empty);
        }

        /// <summary>
        /// Parses the specified FredHopper Universe into a DD4T Component list
        /// </summary>
        /// <param name="extendedPropertyList">
        /// A comma separating list of FredHopper attributes to be inserted into the DD4T Component XML
        /// before deserialisation.
        /// e.g. "commentcount,rating,commenturl".
        /// Can be null.
        /// </param>
        /// <returns>
        /// A generic List of type Component (DD4T Component)
        /// </returns>
        protected List<Component> ParseUniverse(string extendedPropertyList)
        {
            _logger.Debug("DD4TComponents > ParseUniverse >>>>>>>>");
            _logger.DebugFormat("extendedPropertyList = {0}", extendedPropertyList);

            List<Component> components = new List<Component>();
            XmlDocument xml = null;

            // Very lazy sanity check for objects
            try { int i = Universe.itemssection.items.Count(); }
            catch { return components; }


            _logger.DebugFormat("Universe.itemssection.items.Count() = {0}", Universe.itemssection.items.Count());

            // Loop through each item in the FredHopper Universe
            foreach (var item in Universe.itemssection.items)
            {
                _logger.DebugFormat("item = {0}", item.id);

                // Retrieve the XMl serialised DD4T Component
                var itemComponent = item.attribute.SingleOrDefault(a => a.name == "componentpresentation");

                _logger.DebugFormat("itemComponent null? {0}", itemComponent == null);

                // Don't ASSUME that there is a component presentation available!!!
                if (itemComponent == null)
                    continue;                


                // NG - arrays can be null - so in this instance we've found the componentpresentation but its empty?
                if (itemComponent.value == null)
                    continue;
                

                _logger.DebugFormat("Any itemComponent.values? {0}", itemComponent.value.Count() > 0);

                string componentXml = itemComponent.value[0].Value;

                // Check if we need to pull out some extended property attributes (e.g. commentcount, rating, etc)
                // that will appear OUTSIDE of the XML serialised DD4T component.
                if (!string.IsNullOrEmpty(extendedPropertyList))
                {
                    try
                    {
                        // Load the component presentation into an XML document
                        xml = new XmlDocument();
                        xml.LoadXml(componentXml);

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
                            XmlNode xmlItem = xml.CreateElement("item");
                            xmlItem.InnerXml = sb.ToString();
                            
                            // Navigate to the relevant instertion point
                            XmlNode fields = xml.SelectSingleNode("/Component/Fields");
                            
                            // Now insert the new node into the overall DD4T component XML
                            fields.AppendChild(xmlItem);
                        }

                        // Get the outer XML, ready to deserialise into the DD4T object
                        componentXml = xml.OuterXml;

                        _logger.DebugFormat("Final component Xml: {0}", componentXml);
                    }
                    catch (Exception exc) 
                    {
                        _logger.Error("Error adding extended details", exc);
                    }

                }

                try
                {
                    // Deserialise the DD4T component XML string using the Component Factory
                    ComponentFactory factory = new ComponentFactory();
                    Component component = (Component)factory.GetIComponentObject(componentXml);

                    // Finally, add the component to the list to be returned
                    components.Add(component);
                }
                catch (Exception ex)
                {
                    // FAQ 5 FAILING!!!
                    _logger.Error("Error getting DD4T component", ex);
                }

            }

            return components;
        }

    }
}