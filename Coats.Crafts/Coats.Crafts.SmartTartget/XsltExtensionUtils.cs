using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.IO;
using System.Text.RegularExpressions;

namespace Coats.Crafts.SmartTartget
{
    public class XsltExtensionUtils
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public XsltExtensionUtils() { }

        public XPathNodeIterator Parse(string data)
        {
            if (data == null || data.Length == 0)
            {
                data = "<Empty />";
            }

            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data)))
            {
                XPathDocument xPathDocument = new XPathDocument(ms);
                XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
                XPathExpression xPathExpression = xPathNavigator.Compile("/");
                XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(xPathExpression);
                
                // Probably overkill, but I really need to make sure that everything is freed up
                xPathDocument = null;
                xPathNavigator = null;
                xPathExpression = null;
                try { ms.Close(); }
                catch (Exception) { /* Silent catch */ }
                ms.Dispose();
                GC.Collect();
                return xPathNodeIterator;
            }
        }

        public string EncodeData(XPathNodeIterator data)
        {
            data.MoveNext();
            return data.Current.InnerXml;
        }

        public string DateTimeNow()
        {
            return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public string DateTimeMinInt()
        {
            return DateTime.MinValue.ToString("yyyyMMddHHmm");
        }

        public string DateTimeMin()
        {
            return DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public XPathNodeIterator CreateNested(XPathNodeIterator data, int nestedLimit, string locale)
        {
            /*
            The data will be in a format similar to:
      
            <Keyword Description="Baby" Key="PP002" TaxonomyId="tcm:70-4053-512" Path="\Themes\People\Baby (PP)">
              <Id>tcm:70-19215-1024</Id>
              <Title>Baby (PP)</Title>
              <MetadataFields />
            </Keyword>
            <Keyword Description="Women" Key="PP003" TaxonomyId="tcm:70-4053-513" Path="\Themes\People\Women (PP)">
              <Id>tcm:70-19215-1012</Id>
              <Title>Women (PP)</Title>
              <MetadataFields />
            </Keyword>
            <Keyword Description="Men" Key="PP002" TaxonomyId="tcm:70-4053-512" Path="\Themes\People\Men (PP)">
              <Id>tcm:70-19215-1054</Id>
              <Title>Men (PP)</Title>
              <MetadataFields />
            </Keyword>
            <Keyword Description="Wedding" Key="PP002" TaxonomyId="tcm:70-4053-554" Path="\\Themes\\Special occassions\\Wedding (SO)">
              <Id>tcm:70-19215-1067</Id>
              <Title>Wedding (SO)</Title>
              <MetadataFields />
            </Keyword>
      
            We need to create:

            <sets>
              <attribute identifier="70_themes" type="set">
                <name locale="en-US">Themes</name>
                <value identifier="70_themes__dir__people">People</value>
                <value identifier="70_themes__dir__special_occassions">Special Occassions</value>
              </attribute>
              <attribute identifier="70_themes__dir__people" type="set">
                <name locale="en-US">People</name>
                <value identifier="70_themes__dir__people__dir__baby_(pp)">Baby</value>
                <value identifier="70_themes__dir__people__dir__women_(pp)">Women</value>
                <value identifier="70_themes__dir__people__dir__men_(pp)">Men</value>
              </attribute>
              <attribute identifier="70_themes__dir__special_occassions" type="set">
                <name locale="en-US">Special Occasions</name>
                <value identifier="70_themes__dir__special_occassions__dir__wedding">Wedding</value>
              </attribute>      
            </sets>
            */

            Dictionary<string, string> dic = new Dictionary<string, string>();
            int i, j;

            foreach (XPathNavigator item in data)
            {
                string itemPath = item.GetAttribute("Path", "");
                string itemPubId = item.GetAttribute("TaxonomyId", "");
                string itemPathUsingDesc = item.GetAttribute("PathUsingDescription", "");

                if (string.IsNullOrEmpty(itemPath) || string.IsNullOrEmpty(itemPubId))
                {
                    continue;
                }

                if (itemPath.StartsWith("\\")) { itemPath = itemPath.Remove(0, 1); }
                if (itemPathUsingDesc.StartsWith("\\")) { itemPathUsingDesc = itemPathUsingDesc.Remove(0, 1); }
                string[] paths = itemPath.Split('\\');
                string[] pathDescs = itemPathUsingDesc.Split('\\');

                // "tcm:70-4053-512"
                // We just want the "70"
                itemPubId = itemPubId.Split(':')[1].Split('-')[0];

                int limit = Math.Min(paths.Length, nestedLimit + 1);
                for (i = 0; i < limit; i++)
                {
                    string key = itemPubId + "_" + paths[0];
                    for (j = 1; j <= i; j++)
                    {
                        key = key + "\\" + paths[j];
                    }

                    // Covert to lower case, and replace all non alpha, underscore, and back slashes with underscores.
                    Regex rgx = new Regex("[^a-z0-9_\\\\]"); // We need FOUR back slashes because the escaping needs escaping!
                    key = rgx.Replace(key.ToLower(), "_");

                    rgx = null; // Overkill free-up of memory!

                    //string itemDesc = descsitem.GetAttribute("Description", "");
                    string itemDesc = string.Empty;
                    try { itemDesc = pathDescs[i]; }
                    catch (Exception) { itemDesc = string.Empty; }

                    string val = System.Net.WebUtility.HtmlEncode(itemDesc.Length > 0 ? itemDesc : paths[i]);
                    try
                    {
                        dic.Add(key, val);
                    }
                    catch (Exception) { }
                }

            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<sets>");

            i = 1;
            do
            {
                // Get parent keys
                var items = dic.Where(k => k.Key.Split('\\').Count() == i);
                if (items != null && items.Count() > 0)
                {
                    foreach (var item in items)
                    {
                        var innerItems = dic.Where(k => ((k.Key.Split('\\').Count() == i + 1) && k.Key.StartsWith(item.Key + "\\")));
                        if (innerItems != null && innerItems.Count() > 0)
                        {
                            /*
                            <attribute identifier="themes" type="set">
                              <name locale="en-US">Themes</name>
                              <value identifier="themes__dir__people">People</value>
                              <value identifier="themes__dir__special_occassions">Special occassions</value>
                            </attribute>
                            */
                            sb.AppendFormat("<attribute identifier=\"{0}\" type=\"set\">", item.Key.Replace("\\", "__dir__"));
                            sb.AppendFormat("<name locale=\"{0}\">{1}</name>", locale, item.Value);
                            foreach (var innerItem in innerItems)
                            {
                                sb.AppendFormat("<value identifier=\"{0}\">{1}</value>", innerItem.Key.Replace("\\", "__dir__"), innerItem.Value);
                            }
                            sb.Append("</attribute>");
                        }
                    }
                    i++;
                }
                else
                {
                    break;
                }
            } while (true);

            sb.Append("</sets>");

            using (StringReader stringReader = new StringReader(sb.ToString()))
            {
                XPathDocument xPathDocument = new XPathDocument(stringReader);
                XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
                XPathExpression xPathExpression = xPathNavigator.Compile("/");
                XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(xPathExpression);
                
                // Probably overkill, but I really need to make sure that everything is freed up
                stringReader.Close();
                stringReader.Dispose();
                dic.Clear();
                dic = null;
                sb.Clear();
                sb = null;
                xPathDocument = null;
                xPathNavigator = null;
                xPathExpression = null;
                GC.Collect();
                return xPathNodeIterator;
            }
        }
    }    
}
